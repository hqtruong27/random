namespace Infrastructure;

public class AmbientContext
{
    private static readonly AsyncLocal<AmbientContext> _current = new();

    public static AmbientContext Current
    {
        get => _current.Value ??= new();
        set => _current.Value = value;
    }

    public IServiceProvider ServiceProvider { get; init; } = null!;
    public IDomainEventDispatcher DomainEvent
        => ServiceProvider.GetRequiredService<IDomainEventDispatcher>();

    public IEventPublisher Event
        => ServiceProvider.GetRequiredService<IEventPublisher>();
}
