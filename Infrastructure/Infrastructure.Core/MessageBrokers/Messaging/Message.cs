namespace Infrastructure.MessageBrokers.Messaging;

public interface IMessage
{
    string Body { get; }
    string EventType { get; }
}

public class MessageWrapper
{
    public string Body { get; set; } = default!;
    public string EventType { get; set; } = default!;
    public int Order { get; set; }
}

public class SnsMessageResponse
{
    public string Type { get; set; } = default!;
    public string MessageId { get; set; } = default!;
    public string TopicArn { get; set; } = default!;
    public string Subject { get; set; } = default!;
    public string Message { get; set; } = default!;
    public string Timestamp { get; set; } = default!;
    public string Value { get; set; } = default!;
}
