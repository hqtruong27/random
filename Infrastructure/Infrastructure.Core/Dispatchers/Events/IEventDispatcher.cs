namespace Infrastructure.Dispatchers;

public interface IEventDispatcher
{
    Task Dispatch(IMessage message, CancellationToken cancellationToken);
}

public class DelegateEventDispatcher(EventTypeResolver eventTypeResolver) : IEventDispatcher
{
    public async Task Dispatch(IMessage message, CancellationToken cancellationToken)
    {
        var (handlers, @event) = eventTypeResolver.Resolve(message);
        foreach (var handler in handlers)
        {
            await handler(@event, cancellationToken);
        }
    }
}

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class EventAttribute(string name) : Attribute
{
    public string Name { get; } = name;
    public int Order { get; set; }
}