using System.Threading.Channels;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Dispatchers;

public class ChannelEventReader(
    Channel<Func<IServiceProvider, CancellationToken, Task>> channel,
    IServiceProvider serviceProvider,
    ILogger<ChannelEventReader> logger) : BackgroundService
{

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("ChannelEventReader is starting.");

        await foreach (var workItem in channel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                logger.LogInformation("Processing event from channel...");

                await workItem(serviceProvider, stoppingToken);

                logger.LogInformation("Event processed successfully.");
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("ChannelEventReader is stopping due to cancellation request.");
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing event from channel.");
            }
        }

        logger.LogInformation("ChannelEventReader is stopping.");
    }
}
