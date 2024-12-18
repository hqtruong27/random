using Microsoft.Extensions.Hosting;

namespace Infrastructure.Dispatchers;

public class EventSubscriberBackgroundService(
    IEventDispatcher eventDispatcher,
    IEventSubscriber messageReceiver) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await messageReceiver.SubscribeAsync(eventDispatcher.Dispatch, stoppingToken);
    }
}
