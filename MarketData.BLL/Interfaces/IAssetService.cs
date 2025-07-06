using MarketData.BLL.Models.DtoModels;
using MarketData.BLL.Models.Requests;
using MarketData.BLL.Models.Responses;

namespace MarketData.BLL.Interfaces;

public interface IAssetService
{
    Task<List<AssetDto>> GetAssets();
    Task<Result<List<AssetDto>>> GetAssetsFromApiAsync(string accessToken);
    Task<Result<List<AssetDto>>> SyncAssetsAsync(string accessToken);
    Task<Result<List<PriceResponseDto>>> GetPricesAsync(List<PriceRequestDto> requests);

}