using MarketData.BLL.Models.DtoModels;

namespace MarketData.BLL.Interfaces;

public interface IAssetService
{
    Task<List<AssetDto>> GetAssetsAsync();
}