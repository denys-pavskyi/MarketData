using MarketData.DAL.Configurations;
using MarketData.DAL.Interfaces;

namespace MarketData.DAL.Repositories;

public class AssetRepository: IAssetRepository
{
    private readonly AppDbContext _context;

    public AssetRepository(AppDbContext context)
    {
        _context = context;
    }




}