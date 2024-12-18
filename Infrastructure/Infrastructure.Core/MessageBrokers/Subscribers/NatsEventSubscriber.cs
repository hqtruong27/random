using NATS.Client;

namespace Infrastructure.MessageBrokers;

public class NatsEventSubscriber(
    IConnection connection,
    NatsOptions options,
    ILogger<NatsEventSubscriber> logger) : IEventSubscriber, IDisposable
{

    public Task SubscribeAsync(Func<IMessage, CancellationToken, Task> handler, CancellationToken cancellationToken)
    {
        try
        {
            var subscription = connection.SubscribeAsync(options.Subject, async (sender, args) =>
            {
                try
                {
                    var message = Encoding.UTF8.GetString(args.Message.Data);
                    var @event = JsonSerializer.Deserialize<NatsMessage>(message)!;

                    logger.LogInformation(
                        "Event received from NATS subject '{Subject}': {EventType}",
                        options.Subject,
                        @event.GetType().Name
                        );

                    await handler(@event, cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to process event from NATS");
                }

            });

            logger.LogInformation("Subscribed to NATS subject: {Subject}", options.Subject);

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to subscribe to NATS");
            throw;
        }
    }

    public void Dispose()
    {
        connection?.Dispose();
        GC.SuppressFinalize(this);
    }
}
