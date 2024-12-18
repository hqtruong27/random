namespace Infrastructure.Dispatchers;

public abstract class DomainEventHandler<TEvent> : IDomainEventHandler<TEvent>
    where TEvent : IDomainEvent
{
    public abstract Task Handle(TEvent @event, CancellationToken cancellationToken);
}
