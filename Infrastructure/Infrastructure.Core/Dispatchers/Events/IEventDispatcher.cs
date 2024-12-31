namespace Infrastructure.Dispatchers;

public interface IEventDispatcher
{
    Task<bool> Dispatch(IMessage message, CancellationToken cancellationToken);
}

public class DelegateEventDispatcher(EventTypeResolver eventTypeResolver) : IEventDispatcher
{
    public async Task<bool> Dispatch(IMessage message, CancellationToken cancellationToken)
    {
        var (handlers, @event) = eventTypeResolver.Resolve(message);
        if (handlers is null)
        {
            return false;
        }

        foreach (var handler in handlers)
        {
            await handler(@event, cancellationToken);
        }

        return true;
    }
}

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class EventAttribute(string name) : Attribute
{
    public string Name { get; } = name;
    public int Order { get; set; }
}