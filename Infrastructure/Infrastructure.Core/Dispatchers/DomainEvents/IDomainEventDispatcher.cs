namespace Infrastructure.Dispatchers;

public interface IDomainEventDispatcher
{
    Task Raise<T>(T @event) where T : IDomainEvent;
}
