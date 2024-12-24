namespace Infrastructure;

internal class EventContextAccessor
{
    private static readonly AsyncLocal<EventContextAccessor> _current = new();

    public static EventContextAccessor Current
    {
        get => _current.Value ?? new();
        set => _current.Value = value;
    }

    public IServiceProvider ServiceProvider { get; init; } = null!;
    public IDomainEventDispatcher DomainEvent
        => ServiceProvider.GetRequiredService<IDomainEventDispatcher>();

    public IEventPublisher Event
        => ServiceProvider.GetRequiredService<IEventPublisher>();

    public IDbContext Context
       => ServiceProvider.GetRequiredService<IDbContext>();
}
