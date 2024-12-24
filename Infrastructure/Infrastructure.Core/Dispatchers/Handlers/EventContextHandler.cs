namespace Infrastructure.Dispatchers;

public abstract class EventContextHandler
{
    protected static IDomainEventDispatcher DomainEvent => EventContextAccessor.Current.DomainEvent;
    protected static IEventPublisher Event => EventContextAccessor.Current.Event;
    protected static IDbContext Context => EventContextAccessor.Current.Context;
}
