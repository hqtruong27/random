namespace Infrastructure.Dispatchers;

public class EventTypeResolver(Dictionary<string, Type> eventTypes)
{
    private readonly Dictionary<Type, List<Func<object, CancellationToken, Task>>> _eventHandlers = [];

    // --- Methods for Managing Event Types ---

    public void RegisterEventType(string route, Type eventType)
    {
        eventTypes[route] = eventType;
    }

    public Type? GetEventType(string route)
    {
        eventTypes.TryGetValue(route, out var eventType);
        return eventType;
    }

    public bool HasEventType(string route)
    {
        return eventTypes.ContainsKey(route);
    }

    public IEnumerable<string> GetRegisteredRoutes()
    {
        return eventTypes.Keys;
    }

    public IEnumerable<Type> GetRegisteredEventTypes()
    {
        return eventTypes.Values;
    }

    // --- Methods for Managing Event Handlers ---

    public void RegisterHandler(Type eventType, Func<object, CancellationToken, Task> handler)
    {
        if (!_eventHandlers.TryGetValue(eventType, out var value))
        {
            value = [];
            _eventHandlers[eventType] = value;
        }

        value.Add(handler);
    }

    public void RegisterHandlers(Type eventType, IEnumerable<Func<object, CancellationToken, Task>> handlers)
    {
        if (!_eventHandlers.TryGetValue(eventType, out var value))
        {
            value = [];
            _eventHandlers[eventType] = value;
        }

        value.AddRange(handlers);
    }

    public bool HasHandlers(Type eventType)
    {
        return _eventHandlers.ContainsKey(eventType);
    }

    public int GetHandlerCount(Type eventType)
    {
        return _eventHandlers.TryGetValue(eventType, out var handlers) ? handlers.Count : 0;
    }

    // --- Method for Resolving Handlers and Payload ---

    public (IEnumerable<Func<object, CancellationToken, Task>> Handlers, object Event) Resolve(IMessage message)
    {
        var eventType = GetEventType(message.EventType);
        if (eventType == null)
        {
            Console.WriteLine($"Unknown event type: {message.EventType}");
            return ([], new());
        }

        if (!_eventHandlers.TryGetValue(eventType, out var handlers))
        {
            Console.WriteLine($"No handlers registered for event type: {message.EventType}");
            return ([], new());
        }

        object @event;
        try
        {
            @event = JsonSerializer.Deserialize(message.Body, eventType)!;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error de-serializing event data for event type {message.EventType}: {ex.Message}");
            return ([], new());
        }

        return (handlers, @event);
    }
}