using AutoMapper;
using MarketData.BLL.Interfaces;
using MarketData.DAL.Interfaces;

namespace MarketData.BLL.Services;

public class AssetService: IAssetService
{
    
    private readonly IAssetRepository _assetRepository;
    private readonly IAssetSyncMetadataRepository _metadataRepository;
    private readonly IMapper _mapper;


    public AssetService(IAssetRepository assetRepository, 
        IAssetSyncMetadataRepository metadataRepository, IMapper mapper)
    {
        _assetRepository = assetRepository;
        _metadataRepository = metadataRepository;
        _mapper = mapper;
    }



}