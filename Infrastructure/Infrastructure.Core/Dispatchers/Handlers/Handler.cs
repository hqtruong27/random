namespace Infrastructure.Dispatchers;

public abstract class Handler
{
    protected static IDomainEventDispatcher DomainEvent => AmbientContext.Current.DomainEvent;
    protected static IEventPublisher Event => AmbientContext.Current.Event;
}
