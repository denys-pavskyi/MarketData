using MarketData.BLL.Models.Other;
using MarketData.BLL.Models.Responses;

namespace MarketData.BLL.Interfaces;

public interface IWebSocketPriceService
{
    Task<PriceResponseDto> GetCurrentPriceAsync(string instrumentId, string provider, CancellationToken cancellationToken = default);
}