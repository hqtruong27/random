namespace Infrastructure.MessageBrokers;

public class SqsEventSubscriber(IAmazonSQS sqsClient, AwsOptions options) : IEventSubscriber
{
    public async Task SubscribeAsync(Func<IMessage, CancellationToken, Task<bool>> handler, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var request = new ReceiveMessageRequest
            {
                QueueUrl = options.SqsQueueUrl,
                MaxNumberOfMessages = 10,
                WaitTimeSeconds = 20,
                MessageAttributeNames = ["All"]
            };

            var response = await sqsClient.ReceiveMessageAsync(request, cancellationToken);

            foreach (var message in response.Messages)
            {
                if (!await handler(new SqsMessage(message), cancellationToken))
                {
                    continue;
                }

                await sqsClient.DeleteMessageAsync(
                    options.SqsQueueUrl,
                    message.ReceiptHandle,
                    cancellationToken
                    );
            }
        }
    }
}
