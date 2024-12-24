namespace Infrastructure;

internal class DomainEventContextAccessor
{
    private static readonly AsyncLocal<DomainEventContextAccessor> _current = new();

    public static DomainEventContextAccessor Current
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
