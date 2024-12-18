namespace Infrastructure.MessageBrokers;

public interface IEventSubscriber
{
    Task SubscribeAsync(Func<IMessage, CancellationToken, Task> handler, CancellationToken cancellationToken);
}
