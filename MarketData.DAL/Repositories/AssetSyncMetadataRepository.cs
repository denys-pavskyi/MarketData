using MarketData.DAL.Configurations;
using MarketData.DAL.Interfaces;

namespace MarketData.DAL.Repositories;

public class AssetSyncMetadataRepository: IAssetSyncMetadataRepository
{
    private readonly AppDbContext _context;

    public AssetSyncMetadataRepository(AppDbContext context)
    {
        _context = context;
    }



}