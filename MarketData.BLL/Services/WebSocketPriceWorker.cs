using MarketData.BLL.Interfaces;
using MarketData.BLL.Models.Responses;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text;
using System.Threading.Channels;
using MarketData.BLL.Helpers;
using Microsoft.Extensions.Configuration;

namespace MarketData.BLL.Services;

public class WebSocketPriceWorker : BackgroundService
{
    private readonly IPriceCacheService _cache;
    private readonly IAuthService _authService;
    private readonly ILogger<WebSocketPriceWorker> _logger;
    private readonly IConfiguration _configuration;


    private ClientWebSocket _ws;
    private readonly Channel<(string instrumentId, string provider)> _subscriptionQueue = Channel.CreateUnbounded<(string, string)>();

    public WebSocketPriceWorker(
        IPriceCacheService cache,
        IAuthService authService,
        ILogger<WebSocketPriceWorker> logger, IConfiguration configuration)
    {
        _cache = cache;
        _authService = authService;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task SubscribeAsync(string instrumentId, string provider)
    {
        if (!_cache.IsSubscribed(instrumentId, provider))
        {
            _cache.RegisterSubscription(instrumentId, provider);
            await _subscriptionQueue.Writer.WriteAsync((instrumentId, provider));
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ConnectAndListenAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WebSocket connection error. Reconnecting in 5s...");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    private async Task ConnectAndListenAsync(CancellationToken cancellationToken)
    {
        var token = (await _authService.GetAccessTokenAsync()).Value;

        _ws = new ClientWebSocket();

        var baseUri = _configuration["Fintacharts:WebSocketUri"];
        var wsUri = new Uri($"{baseUri}/api/streaming/ws/v1/realtime?token={token}");
        await _ws.ConnectAsync(wsUri, cancellationToken);

        _logger.LogInformation("WebSocket connected.");

        var receiveTask = ReceiveLoop(cancellationToken);
        var subscribeTask = SubscriptionLoop(cancellationToken);

        await Task.WhenAny(receiveTask, subscribeTask);
    }

    private async Task SubscriptionLoop(CancellationToken cancellationToken)
    {
        await foreach (var (instrumentId, provider) in _subscriptionQueue.Reader.ReadAllAsync(cancellationToken))
        {
            var msg = new
            {
                type = "l1-subscription",
                id = Guid.NewGuid().ToString(),
                instrumentId,
                provider,
                subscribe = true,
                kinds = new[] { "ask", "bid", "last" }
            };

            var json = JsonSerializer.Serialize(msg);
            var buffer = Encoding.UTF8.GetBytes(json);
            await _ws.SendAsync(buffer, WebSocketMessageType.Text, true, cancellationToken);

            _logger.LogInformation($"Subscribed to {instrumentId} ({provider})");
        }
    }

    private async Task ReceiveLoop(CancellationToken cancellationToken)
    {
        var buffer = new byte[8192];

        while (_ws.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
        {
            var result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
            _logger.LogInformation("Received: " + Encoding.UTF8.GetString(buffer, 0, result.Count));


            if (result.MessageType == WebSocketMessageType.Close)
            {
                _logger.LogWarning("WebSocket closed by server.");
                break;
            }

            var json = Encoding.UTF8.GetString(buffer, 0, result.Count);
            if (json.Contains("l1-update") || json.Contains("l1-snapshot"))
            {
                try
                {
                    var doc = JsonDocument.Parse(json);
                    var root = doc.RootElement;

                    var price = PriceParserHelper.ParsePriceFromWebSocket(root);
                    if (price != null)
                    {
                        _cache.UpdatePrice(price);
                        _logger.LogInformation($"Parsed price for {price.InstrumentId}: {price.Price} @ {price.UpdateTime}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to parse l1-update message");
                }
            }
        }
    }

    public async Task UnsubscribeAsync(string instrumentId, string provider)
    {
        var msg = new
        {
            type = "l1-subscription",
            id = Guid.NewGuid().ToString(),
            instrumentId,
            provider,
            subscribe = false,
            kinds = new[] { "ask", "bid", "last" }
        };

        var json = JsonSerializer.Serialize(msg);
        var buffer = Encoding.UTF8.GetBytes(json);

        await _ws.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);

        _logger.LogInformation($"Unsubscribed from {instrumentId} ({provider})");

        _cache.RemoveSubscription(instrumentId, provider);
    }

}