using MarketData.BLL.Models.Responses;

namespace MarketData.BLL.Interfaces;

public interface IPriceCacheService
{
    Task<PriceResponseDto> GetOrWaitForPriceAsync(string instrumentId, string provider, CancellationToken cancellationToken = default);
    void UpdatePrice(PriceResponseDto price);
    bool IsSubscribed(string instrumentId, string provider);
    void RegisterSubscription(string instrumentId, string provider);
    IEnumerable<(string instrumentId, string provider)> GetAllSubscribed();

    void RemoveSubscription(string instrumentId, string provider);
}