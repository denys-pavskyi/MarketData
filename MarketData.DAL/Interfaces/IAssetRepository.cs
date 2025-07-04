using MarketData.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace MarketData.DAL.Interfaces;

public interface IAssetRepository
{
    Task<List<Asset>> GetAllAsync();
    Task AddAsync(Asset asset);
    Task UpdateAsync(Asset asset);

    Task DeleteAsync(Asset asset);

    Task AddMappingAsync(AssetMapping mapping);

    Task UpdateMappingAsync(AssetMapping mapping);

    Task DeleteMappingAsync(AssetMapping mapping);
}