using MarketData.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace MarketData.DAL.Interfaces;

public interface IAssetRepository
{
    Task<List<Asset>> GetAllAsync();
    Task UpdateAsync(Asset asset);

    Task DeleteAsync(Asset asset);
}