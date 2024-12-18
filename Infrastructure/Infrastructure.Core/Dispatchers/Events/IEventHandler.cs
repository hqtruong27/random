namespace Infrastructure.Dispatchers;

public interface IEventHandler<in TEvent>  where TEvent : IEvent
{
    Task Handle(TEvent @event, CancellationToken cancellationToken);
}