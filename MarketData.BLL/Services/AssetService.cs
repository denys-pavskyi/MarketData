using AutoMapper;
using MarketData.BLL.Interfaces;
using MarketData.BLL.Models.DtoModels;
using MarketData.BLL.Models.Requests;
using MarketData.BLL.Models.Responses;
using MarketData.DAL.Entities;
using MarketData.DAL.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Web;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace MarketData.BLL.Services;

public class AssetService: IAssetService
{
    private readonly HttpClient _httpClient;
    private readonly IAssetRepository _assetRepository;
    private readonly IMapper _mapper;

    private readonly WebSocketPriceWorker _wsWorker;
    private readonly IPriceCacheService _cache;
    private readonly ILogger<AssetService> _logger;
    private readonly IAuthService _authService;
    private readonly IConfiguration _configuration;

    public AssetService(IAssetRepository assetRepository, 
        IMapper mapper,
        IHttpClientFactory httpClientFactory, 
        WebSocketPriceWorker wsWorker, IPriceCacheService cache, ILogger<AssetService> logger, 
        IAuthService authService, IConfiguration configuration)
    {
        _assetRepository = assetRepository;
        _mapper = mapper;
        _wsWorker = wsWorker;
        _cache = cache;
        _logger = logger;
        _authService = authService;
        _configuration = configuration;
        _httpClient = httpClientFactory.CreateClient("ApiClient");
    }

    public async Task<Result<List<AssetDto>>> GetAssetsFromApiAsync(string accessToken)
    {
        var pageSize = 100;

        var allAssets = new List<AssetDto>();


        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);


        var firstUri = $"/api/instruments/v1/instruments?page=1&size={pageSize}";
        var firstPage = await GetPageAsync(firstUri);

        if (!firstPage.IsSuccess)
        {
            return Result<List<AssetDto>>.Failure(firstPage.Error);
        }
            

        allAssets.AddRange(firstPage.Value!.Data);
        var totalPages = firstPage.Value.Paging.Pages;

        if (totalPages > 1)
        {
            var tasks = new List<Task<Result<PagedResponseDto<AssetDto>>>>();

            for (int page = 2; page <= totalPages; page++)
            {
                var uri = $"/api/instruments/v1/instruments?page={page}&size={pageSize}";
                tasks.Add(GetPageAsync(uri));
            }

            var results = await Task.WhenAll(tasks);

            foreach (var result in results)
            {
                if (!result.IsSuccess)
                {
                    return Result<List<AssetDto>>.Failure(result.Error);
                }

                allAssets.AddRange(result.Value!.Data);
            }
        }

        return Result<List<AssetDto>>.Success(allAssets);
    }

    public async Task<List<AssetDto>> GetAssets()
    {
        var assetsFromDb = await _assetRepository.GetAllAsync();
        var assetDtos = _mapper.Map<List<AssetDto>>(assetsFromDb);

        return assetDtos;
    }


    public async Task<Result<List<AssetDto>>> SyncAssetsAsync(string accessToken)
    {
        // Assets from Fintacharts API
        var apiResult = await GetAssetsFromApiAsync(accessToken);
        if (!apiResult.IsSuccess)
        {
            return Result<List<AssetDto>>.Failure(apiResult.Error);
        }
        var allAssets = apiResult.Value;

        // Assets from db
        var existingAssets = await _assetRepository.GetAllAsync();
        var existingAssetIds = existingAssets.Select(a => a.Id).ToHashSet();

        // Comparing existing assets
        foreach (var assetDto in allAssets)
        {
            await SynchronizeAssetAsync(assetDto, existingAssets, existingAssetIds);
        }

        // Remove assets no longer existent
        foreach (var obsoleteAssetId in existingAssetIds)
        {
            var obsoleteAsset = existingAssets.First(a => a.Id == obsoleteAssetId);
            await _assetRepository.DeleteAsync(obsoleteAsset);
        }

        return Result<List<AssetDto>>.Success(allAssets);
    }

    public async Task<Result<List<PriceResponseDto>>> GetPricesAsync(List<PriceRequestDto> requests)
    {
        var prices = new List<PriceResponseDto>();
        var accessToken = (await _authService.GetAccessTokenAsync()).Value!;

        // Actual data

        foreach (var req in requests)
        {
            try
            {
                await _wsWorker.SubscribeAsync(req.InstrumentId, req.Provider);
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

                var price = await _cache.GetOrWaitForPriceAsync(req.InstrumentId, req.Provider, cts.Token);
                prices.Add(price);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning($"Timeout waiting for price of {req.InstrumentId} ({req.Provider})");
                prices.Add(new PriceResponseDto
                {
                    InstrumentId = req.InstrumentId,
                    Provider = req.Provider,
                    Price = null,
                    UpdateTime = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while getting price for {req.InstrumentId}");
                prices.Add(new PriceResponseDto
                {
                    InstrumentId = req.InstrumentId,
                    Provider = req.Provider,
                    Price = null,
                    UpdateTime = DateTime.UtcNow
                });
            }

            await _wsWorker.UnsubscribeAsync(req.InstrumentId, req.Provider);
        }


        // Historic data

        foreach (var price in prices.Where(p => p.Price != null))
        {
            var req = requests.FirstOrDefault(r =>
                r.InstrumentId == price.InstrumentId &&
                r.Provider == price.Provider);

            if (req == null) continue;

            try
            {
                var bars = await GetHistoricalBarsAsync(req, accessToken);
                price.HistoricalBars = bars;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while fetching historical bars for {price.InstrumentId}");
                price.HistoricalBars = new List<BarDto>();
            }
        }

        return Result<List<PriceResponseDto>>.Success(prices);

    }

    private async Task<List<BarDto>> GetHistoricalBarsAsync(PriceRequestDto request, string accessToken)
    {

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var baseUri = _configuration["Fintacharts:WebSocketUri"];
        var uriBuilder = new UriBuilder($"{baseUri}/api/bars/v1/bars/count-back");
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);

        query["instrumentId"] = request.InstrumentId;
        query["provider"] = request.Provider;
        query["interval"] = request.Interval.ToString();
        query["periodicity"] = request.Periodicity;
        query["barsCount"] = request.BarsCount.ToString();

        uriBuilder.Query = query.ToString();

        var response = await _httpClient.GetAsync(uriBuilder.Uri);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning($"Failed to fetch historical bars: {response.StatusCode}");
            return new List<BarDto>();
        }

        var content = await response.Content.ReadAsStringAsync();
        var json = JObject.Parse(content);
        var bars = json["data"]?.ToObject<List<BarDto>>() ?? new();

        return bars;
    }

    private async Task SynchronizeAssetAsync(AssetDto assetDto, List<Asset> existingAssets, HashSet<Guid> existingAssetIds)
    {
        var asset = existingAssets.FirstOrDefault(a => a.Id == assetDto.Id);
        if (asset == null)
        {
            asset = _mapper.Map<Asset>(assetDto);
            await _assetRepository.AddAsync(asset);
        }
        else
        {
            _mapper.Map(assetDto, asset);
            await _assetRepository.UpdateAsync(asset);
            existingAssetIds.Remove(asset.Id);
        }

        var existingMappings = asset.Mappings.ToList();
        var existingMappingProviders = existingMappings.Select(m => m.Provider).ToHashSet();

        foreach (var mappingDto in assetDto.Mappings)
        {
            var mapping = existingMappings.FirstOrDefault(m => m.Provider == mappingDto.Provider);
            if (mapping == null)
            {
                mapping = _mapper.Map<AssetMapping>(mappingDto);
                mapping.AssetId = asset.Id;
                await _assetRepository.AddMappingAsync(mapping);
            }
            else
            {
                _mapper.Map(mappingDto, mapping);
                await _assetRepository.UpdateMappingAsync(mapping);
                existingMappingProviders.Remove(mapping.Provider);
            }
        }

        foreach (var obsoleteMapping in existingMappings.Where(m => existingMappingProviders.Contains(m.Provider)))
        {
            await _assetRepository.DeleteMappingAsync(obsoleteMapping);
        }
    }

    private async Task<Result<PagedResponseDto<AssetDto>>> GetPageAsync(string uri)
    {
        var response = await _httpClient.GetAsync(uri);

        if (!response.IsSuccessStatusCode)
        {
            var error = new ErrorResponse
            {
                HttpCode = response.StatusCode,
                Message = response.StatusCode == HttpStatusCode.Unauthorized
                    ? "Access token is invalid or expired"
                    : "Failed to fetch asset data"
            };

            return Result<PagedResponseDto<AssetDto>>.Failure(error);
        }

        var stream = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<PagedResponseDto<AssetDto>>(
            stream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return result is null
            ? Result<PagedResponseDto<AssetDto>>.Failure(new ErrorResponse
            {
                HttpCode = HttpStatusCode.InternalServerError,
                Message = "Failed to parse asset data"
            })
            : Result<PagedResponseDto<AssetDto>>.Success(result);
    }

}