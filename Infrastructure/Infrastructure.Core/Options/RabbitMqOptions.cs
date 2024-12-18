namespace Infrastructure.Options;

public class RabbitMqOptions
{
    public string ConnectionString { get; set; } = default!;
    public string ExchangeName { get; set; } = default!;
    public string QueueName { get; set; } = default!;
}
