namespace Infrastructure.Options;

public class GooglePubSubOptions
{
    public string ProjectId { get; set; } = default!;
    public string TopicId { get; set; } = default!;
    public string SubscriptionId { get; set; } = default!;
    public string CredentialsFilePath { get; set; } = default!; // Optional: Path to JSON credentials file
}
