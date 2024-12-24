namespace Infrastructure.Dispatchers;

public class DomainEventContextHandler
{
    protected static IDomainEventDispatcher DomainEvent => DomainEventContextAccessor.Current.DomainEvent;
    protected static IEventPublisher Event => DomainEventContextAccessor.Current.Event;
    protected static IDbContext Context => DomainEventContextAccessor.Current.Context;
}
