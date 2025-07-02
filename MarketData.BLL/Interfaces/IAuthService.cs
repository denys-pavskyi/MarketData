using MarketData.BLL.Models.Responses;

namespace MarketData.BLL.Interfaces;

public interface IAuthService
{
    Task<Result<string>> GetAccessTokenAsync();
}