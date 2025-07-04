using MarketData.BLL.Models.DtoModels;
using MarketData.BLL.Models.Responses;

namespace MarketData.BLL.Interfaces;

public interface IAssetService
{
    Task<Result<List<AssetDto>>> GetAssetsAsync(string accessToken);
}