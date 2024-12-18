namespace Infrastructure.Dispatchers;

public interface IDomainEventHandler<in TEvent> : INotificationHandler<TEvent> 
    where TEvent : IDomainEvent;