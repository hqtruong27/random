namespace Infrastructure.Options;

public class NatsOptions
{
    public string Url { get; set; } = default!; // = "nats://localhost:4222"; // Default NATS URL
    public string Subject { get; set; } = default!; // Subject to publish/subscribe
}
