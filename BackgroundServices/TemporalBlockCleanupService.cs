using CountryBlockingAPI.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CountryBlockingAPI.BackgroundServices;

public class TemporalBlockCleanupService : BackgroundService
{
    private readonly ITemporalBlockRepository _temporalBlockRepository;
    private readonly ILogger<TemporalBlockCleanupService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(5);

    public TemporalBlockCleanupService(
        ITemporalBlockRepository temporalBlockRepository,
        ILogger<TemporalBlockCleanupService> logger)
    {
        _temporalBlockRepository = temporalBlockRepository ?? throw new ArgumentNullException(nameof(temporalBlockRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Temporal Block Cleanup Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Checking for expired temporal blocks...");
                
                // Remove expired blocks
                int removedCount = await _temporalBlockRepository.RemoveExpiredBlocksAsync();
                
                if (removedCount > 0)
                {
                    _logger.LogInformation("Removed {Count} expired temporal blocks", removedCount);
                }
                else
                {
                    _logger.LogInformation("No expired temporal blocks found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while cleaning up expired temporal blocks");
            }

            // Wait for the next check interval
            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("Temporal Block Cleanup Service is stopping.");
    }
}