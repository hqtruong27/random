namespace Infrastructure.Dispatchers;

public abstract class AmbientContextHandler
{
    protected static IDomainEventDispatcher DomainEvent => AmbientContext.Current.DomainEvent;
    protected static IEventPublisher Event => AmbientContext.Current.Event;
    protected static IDbContext Context => AmbientContext.Current.Context;
    protected static HttpContext? HttpContext => AmbientContext.Current.HttpContext;
}
