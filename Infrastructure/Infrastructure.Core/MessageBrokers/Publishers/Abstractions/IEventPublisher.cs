namespace Infrastructure.MessageBrokers;

public interface IEventPublisher
{
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IEvent;
}
