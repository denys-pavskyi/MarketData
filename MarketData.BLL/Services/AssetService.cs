using AutoMapper;
using MarketData.BLL.Interfaces;
using MarketData.BLL.Models.DtoModels;
using MarketData.BLL.Models.Responses;
using MarketData.DAL.Interfaces;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime;
using System.Text.Json;

namespace MarketData.BLL.Services;

public class AssetService: IAssetService
{
    private readonly HttpClient _httpClient;
    private readonly IAssetRepository _assetRepository;
    private readonly IAssetSyncMetadataRepository _metadataRepository;
    private readonly IMapper _mapper;


    public AssetService(IAssetRepository assetRepository, 
        IAssetSyncMetadataRepository metadataRepository, IMapper mapper,
        IHttpClientFactory httpClientFactory)
    {
        _assetRepository = assetRepository;
        _metadataRepository = metadataRepository;
        _mapper = mapper;
        _httpClient = httpClientFactory.CreateClient("ApiClient");
    }

    public async Task<Result<List<AssetDto>>> GetAssetsAsync(string accessToken)
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