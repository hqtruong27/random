using Google.Cloud.PubSub.V1;
using Google.Protobuf;

namespace Infrastructure.MessageBrokers;

public class GooglePubSubEventBus(GooglePubSubOptions options, ILogger<GooglePubSubEventBus> logger) : EventPublisher
{
    protected override async Task PublishAsync(MessageWrapper wrapper, CancellationToken cancellationToken)
    {
        PublisherClient? publisher = null;

        try
        {
            var topicName = TopicName.FromProjectTopic(options.ProjectId, options.TopicId);
            publisher = await PublisherClient.CreateAsync(topicName);

            var message = new PubsubMessage
            {
                Data = ByteString.CopyFromUtf8(JsonSerializer.Serialize(wrapper)),
            };

            // Add metadata for event type
            message.Attributes.Add("EventType", wrapper.EventType);

            var messageId = await publisher.PublishAsync(message);

            logger.LogInformation("Event published to Google Pub/Sub: {EventType}, Message ID: {MessageId}",
                wrapper.EventType, messageId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to publish event to Google Pub/Sub: {EventType}", wrapper.EventType);
            throw;
        }
        finally
        {
            if (publisher != null)
            {
                try
                {
                    // Shutdown is optional but recommended
                    await publisher.ShutdownAsync(TimeSpan.FromSeconds(10)); // Give it some time to shut down
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to shut down publisher for Google Pub/Sub: {EventType}",
                        wrapper.EventType);
                }
            }
        }
    }
}