using Microsoft.Extensions.Hosting;

namespace Infrastructure.Dispatchers;

public class EventSubscriberBackgroundService(
    IEventDispatcher eventDispatcher,
    IEventSubscriber messageReceiver,
    IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await messageReceiver.SubscribeAsync(async (message, cancellationToken) =>
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            EventContextAccessor.Current = new()
            {
                ServiceProvider = scope.ServiceProvider
            };

            try
            {
                await eventDispatcher.Dispatch(message, cancellationToken);
            }
            finally
            {
                EventContextAccessor.Current = null!;
            }

        }, stoppingToken);
    }
}
