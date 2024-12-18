namespace Infrastructure.MessageBrokers;

public class AmazonEventPublisher(
    ILogger<AmazonEventPublisher> logger,
    IAmazonSimpleNotificationService snsClient,
    AwsOptions options) : EventPublisher
{
    protected override async Task PublishAsync(MessageWrapper wrapper, CancellationToken cancellationToken)
    {
        try
        {
            var message = JsonSerializer.Serialize(wrapper);

            var request = new PublishRequest
            {
                TargetArn = options.TopicArn,
                Message = message
            };

            var response = await snsClient.PublishAsync(request, cancellationToken);

            logger.LogInformation("Event published to Amazon SNS: {EventType}", wrapper.EventType);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to publish event to Amazon SNS: {EventType}", wrapper.EventType);
            throw;
        }
    }
}