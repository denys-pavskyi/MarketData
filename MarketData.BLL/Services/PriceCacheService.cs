using MarketData.BLL.Interfaces;
using MarketData.BLL.Models.Responses;
using System.Collections.Concurrent;

namespace MarketData.BLL.Services;

public class PriceCacheService: IPriceCacheService
{
    private readonly ConcurrentDictionary<(string instrumentId, string provider), PriceResponseDto> _prices = new();
    private readonly ConcurrentDictionary<(string instrumentId, string provider), TaskCompletionSource<PriceResponseDto>> _waiting = new();
    private readonly ConcurrentDictionary<(string instrumentId, string provider), byte> _subscriptions = new();

    public Task<PriceResponseDto> GetOrWaitForPriceAsync(string instrumentId, string provider, CancellationToken cancellationToken = default)
    {
        var key = (instrumentId, provider);

        if (_prices.TryGetValue(key, out var cached))
            return Task.FromResult(cached);

        var tcs = new TaskCompletionSource<PriceResponseDto>(TaskCreationOptions.RunContinuationsAsynchronously);

        if (_waiting.TryAdd(key, tcs))
        {
            // new sub
        }

        cancellationToken.Register(() => tcs.TrySetCanceled());

        return tcs.Task;
    }

    public void UpdatePrice(PriceResponseDto price)
    {
        var key = (price.InstrumentId, price.Provider);
        _prices[key] = price;

        if (_waiting.TryRemove(key, out var tcs))
        {
            tcs.TrySetResult(price);
        }
    }

    public void RegisterSubscription(string instrumentId, string provider)
    {
        _subscriptions.TryAdd((instrumentId, provider), 0);
    }

    public bool IsSubscribed(string instrumentId, string provider)
    {
        return _subscriptions.ContainsKey((instrumentId, provider));
    }

    public IEnumerable<(string instrumentId, string provider)> GetAllSubscribed()
    {
        return _subscriptions.Keys;
    }



}