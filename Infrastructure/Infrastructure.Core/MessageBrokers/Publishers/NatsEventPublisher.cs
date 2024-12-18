using NATS.Client;

namespace Infrastructure.MessageBrokers;

public class NatsEventPublisher(IConnection connection, NatsOptions options, ILogger<NatsEventPublisher> logger) : EventPublisher
{
    protected override Task PublishAsync(MessageWrapper wrapper, CancellationToken cancellationToken)
    {
        try
        {
            var message = JsonSerializer.Serialize(wrapper);
            connection.Publish(options.Subject, Encoding.UTF8.GetBytes(message));

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to publish event to NATS: {EventType}", wrapper.EventType);
            throw;
        }
    }
}
