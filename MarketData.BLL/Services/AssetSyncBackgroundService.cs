using MarketData.BLL.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MarketData.BLL.Services;

public class AssetSyncBackgroundService : BackgroundService
{
    private readonly ILogger<AssetSyncBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _interval = TimeSpan.FromHours(12);

    public AssetSyncBackgroundService(
        ILogger<AssetSyncBackgroundService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Asset sync background service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
                var assetService = scope.ServiceProvider.GetRequiredService<IAssetService>();

                var tokenResult = await authService.GetAccessTokenAsync();

                if (tokenResult.IsSuccess)
                {
                    var syncResult = await assetService.SyncAssetsAsync(tokenResult.Value!);

                    if (syncResult.IsSuccess)
                        _logger.LogInformation("Assets synced successfully");
                    else
                        _logger.LogWarning("Sync failed: {Error}", syncResult.Error?.Message);
                }
                else
                {
                    _logger.LogWarning("Auth token retrieval failed: {Error}", tokenResult.Error?.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during asset sync");
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }


}