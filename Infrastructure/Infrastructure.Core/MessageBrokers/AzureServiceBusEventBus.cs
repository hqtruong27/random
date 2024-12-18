using Azure.Messaging.ServiceBus;

namespace Infrastructure.MessageBrokers;

public class AzureServiceBusEventBus : EventPublisher
{
    private readonly AzureServiceBusOptions _options;
    private readonly ServiceBusClient _client;
    private readonly ILogger<AzureServiceBusEventBus> _logger;

    public AzureServiceBusEventBus(AzureServiceBusOptions options, ILogger<AzureServiceBusEventBus> logger)
    {
        _options = options;
        _logger = logger;
        _client = new ServiceBusClient(_options.ConnectionString);
    }

    protected override async Task PublishAsync(MessageWrapper wrapper, CancellationToken cancellationToken)
    {
        ServiceBusSender? sender = null;
        try
        {
            sender = _client.CreateSender(_options.TopicName);
            var message = new ServiceBusMessage(JsonSerializer.Serialize(wrapper));
            await sender.SendMessageAsync(message, cancellationToken);
            _logger.LogInformation("Event published to Azure Service Bus: {EventType}", wrapper.EventType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish event to Azure Service Bus: {EventType}", wrapper.EventType);
            throw;
        }
        finally
        {
            if (sender != null)
            {
                await sender.DisposeAsync();
            }
        }
    }
}
