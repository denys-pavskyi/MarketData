using MarketData.DAL.Configurations;
using MarketData.DAL.Entities;
using MarketData.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MarketData.DAL.Repositories;

public class AssetRepository: IAssetRepository
{
    private readonly AppDbContext _context;

    public AssetRepository(AppDbContext context)
    {
        _context = context;
    }


    public async Task<List<Asset>> GetAllAsync()
    {
        return await _context.Assets
            .Include(a => a.Mappings)
            .ToListAsync();
    }

    public async Task UpdateAsync(Asset asset)
    {
        _context.Assets.Update(asset);
        await _context.SaveChangesAsync();
    }

    public async Task AddAsync(Asset asset)
    {
        _context.Assets.Add(asset);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Asset asset)
    {
        _context.Assets.Remove(asset);
        await _context.SaveChangesAsync();
    }


    // Asset mapping

    public async Task AddMappingAsync(AssetMapping mapping)
    {
        _context.AssetMappings.Add(mapping);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateMappingAsync(AssetMapping mapping)
    {
        _context.AssetMappings.Update(mapping);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteMappingAsync(AssetMapping mapping)
    {
        _context.AssetMappings.Remove(mapping);
        await _context.SaveChangesAsync();
    }


}