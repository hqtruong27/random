namespace Infrastructure.MessageBrokers;

public abstract class EventPublisher : IEventPublisher
{
    protected virtual Task BeforeAsync(MessageWrapper wrapper, CancellationToken cancellationToken) => Task.CompletedTask;
    protected virtual Task AfterAsync(MessageWrapper wrapper, CancellationToken cancellationToken) => Task.CompletedTask;
    protected abstract Task PublishAsync(MessageWrapper wrapper, CancellationToken cancellationToken);

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IEvent
    {
        var message = new MessageWrapper
        {
            Body = JsonSerializer.Serialize(@event),
            EventType = typeof(T).GetCustomAttribute<EventAttribute>()?.Name ?? typeof(T).Name
        };

        await BeforeAsync(message, cancellationToken);
        await PublishAsync(message, cancellationToken);
        await AfterAsync(message, cancellationToken);
    }
}