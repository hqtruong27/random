namespace Infrastructure.MessageBrokers.Messaging;

public class NatsMessage : IMessage
{
    public string EventType { get; set; } = default!;
    public string Body { get; set; } = default!;
}
