using System.Threading.Channels;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Dispatchers;

public class ChannelEventReader(
    Channel<Func<IServiceProvider, CancellationToken, Task>> channel,
    IServiceScopeFactory serviceScopeFactory,
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

                await using (var scope = serviceScopeFactory.CreateAsyncScope())
                {
                    DomainEventContextAccessor.Current = new()
                    {
                        ServiceProvider = scope.ServiceProvider
                    };

                    await workItem(scope.ServiceProvider, stoppingToken);
                }

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
            finally
            {
                DomainEventContextAccessor.Current = null!;
            }
        }

        logger.LogInformation("ChannelEventReader is stopping.");
    }
}
