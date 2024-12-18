namespace Infrastructure.Dispatchers;

public abstract class EventHandler<TEvent> : IEventHandler<TEvent>
 where TEvent : IEvent
{
    public abstract Task Handle(TEvent @event, CancellationToken cancellationToken);
}
