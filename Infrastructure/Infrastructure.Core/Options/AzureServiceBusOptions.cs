namespace Infrastructure.Options;

public class AzureServiceBusOptions
{
    public string ConnectionString { get; set; } = default!;
    public string TopicName { get; set; } = default!;
    public string SubscriptionName { get; set; } = default!;
}
