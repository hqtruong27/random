namespace Infrastructure.Dispatchers
{
    public abstract class Reactor<TEvent> : EventContextHandler, IEventHandler<TEvent> where TEvent : IEvent
    {
        public abstract Task Handle(TEvent @event, CancellationToken cancellationToken);
    }
}

namespace Vague
{
    public abstract class EvenHandler<TEvent> : Reactor<TEvent> where TEvent : IEvent;
}