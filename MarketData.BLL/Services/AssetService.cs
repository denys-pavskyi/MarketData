using AutoMapper;
using MarketData.BLL.Interfaces;
using MarketData.BLL.Models.DtoModels;
using MarketData.DAL.Interfaces;

namespace MarketData.BLL.Services;

public class AssetService: IAssetService
{
    private readonly HttpClient _httpClient;
    private readonly IAssetRepository _assetRepository;
    private readonly IAssetSyncMetadataRepository _metadataRepository;
    private readonly IMapper _mapper;


    public AssetService(IAssetRepository assetRepository, 
        IAssetSyncMetadataRepository metadataRepository, IMapper mapper, 
        HttpClient httpClient)
    {
        _assetRepository = assetRepository;
        _metadataRepository = metadataRepository;
        _mapper = mapper;
        _httpClient = httpClient;
    }

    public Task<List<AssetDto>> GetAssetsAsync()
    {
        throw new NotImplementedException();
    }
}