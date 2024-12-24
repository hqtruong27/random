namespace Infrastructure.Dispatchers;

public interface IDomainEventDispatcher
{
    Task Raise<T>(T @event, CancellationToken cancellationToken = default) where T : IDomainEvent;
}
