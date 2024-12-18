namespace Infrastructure.MessageBrokers.Messaging;

public class SqsMessage : IMessage
{
    public string EventType { get; set; }
    public string Body { get; set; }
    public SqsMessage(Message message)
    {
        try
        {
            var snsMessage = JsonSerializer.Deserialize<SnsMessageResponse>(message.Body);

            if (snsMessage != null)
            {
                var messageWrapper = JsonSerializer.Deserialize<MessageWrapper>(snsMessage.Message);

                Body = messageWrapper?.Body ?? string.Empty;
                EventType = messageWrapper?.EventType ?? string.Empty;
            }
            else
            {
                Body = string.Empty;
                EventType = string.Empty;
            }
        }
        catch //(JsonException ex)
        {
            Body = string.Empty;
            EventType = string.Empty;
            //throw new FormatException("Invalid message format. Failed to deserialize message.", ex);
        }
    }
}
