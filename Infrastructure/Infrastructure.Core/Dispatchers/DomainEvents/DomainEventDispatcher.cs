using System.Threading.Channels;

namespace Infrastructure.Dispatchers;

public class DomainEventDispatcher(
    Channel<Func<IServiceProvider, CancellationToken, Task>> channel,
    ILogger<DomainEventDispatcher> logger) : IDomainEventDispatcher
{
    public async Task Raise<T>(T @event) where T : IDomainEvent
    {
        logger.LogInformation(
            "Raising event: {EventType} (ID: {EventId})",
            typeof(T).Name,
            @event.Id
            );

        await channel.Writer.WriteAsync(async (serviceProvider, cancellationToken) =>
        {
            await using var scope = serviceProvider.CreateAsyncScope();

            try
            {
                var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();
                await publisher.Publish(@event, cancellationToken);

                //var eventStore = scope.ServiceProvider.GetRequiredService<IEventStore>();
                //await eventStore.SaveEventAsync(@event.Id, @event, DateTime.UtcNow, typeof(T).Name);

                logger.LogInformation(
                    "Successfully published event: {EventType} (ID: {EventId})",
                    typeof(T).Name,
                    @event.Id
                    );
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Error publishing event: {EventType} (ID: {EventId})",
                    typeof(T).Name,
                    @event.Id
                    );
            }
        });
    } 
}