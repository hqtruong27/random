using RabbitMQ.Client;

namespace Infrastructure.MessageBrokers;

public class RabbitMQEventBus(RabbitMqOptions options, ILogger<RabbitMQEventBus> logger) : EventPublisher, IAsyncDisposable
{
    private IChannel? _channel;
    private IConnection? _connection;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);

    private async Task EnsureConnectionAsync(CancellationToken cancellationToken)
    {
        await _connectionLock.WaitAsync(cancellationToken);

        try
        {
            if (_connection == null || !_connection.IsOpen)
            {
                var factory = new ConnectionFactory() { Uri = new Uri(options.ConnectionString) };
                _connection = await factory.CreateConnectionAsync(cancellationToken);
                logger.LogInformation("Connected to RabbitMQ.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to connect to RabbitMQ.");
            throw;
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    private async Task EnsureChannelAsync(CancellationToken cancellationToken)
    {
        await _connectionLock.WaitAsync(cancellationToken);
        try
        {
            if (_channel == null || !_channel.IsOpen)
            {
                await EnsureConnectionAsync(cancellationToken);
                _channel = await _connection!.CreateChannelAsync(cancellationToken: cancellationToken);
                await _channel.ExchangeDeclareAsync(exchange: options.ExchangeName, type: ExchangeType.Fanout, durable: true, cancellationToken: cancellationToken);
                await _channel.QueueDeclareAsync(queue: options.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null, cancellationToken: cancellationToken);
                await _channel.QueueBindAsync(queue: options.QueueName, exchange: options.ExchangeName, routingKey: "", cancellationToken: cancellationToken);
                logger.LogInformation("RabbitMQ channel initialized.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to initialize RabbitMQ channel.");
            throw;
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel != null)
        {
            try
            {
                await _channel.CloseAsync();
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to close RabbitMQ channel during DisposeAsync.");
            }

            await _channel.DisposeAsync();
            GC.SuppressFinalize(this);
        }

        if (_connection != null)
        {
            try
            {
                await _connection.CloseAsync();
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to close RabbitMQ connection during DisposeAsync.");
            }

            await _connection.DisposeAsync();
        }
    }

    protected override async Task PublishAsync(MessageWrapper wrapper, CancellationToken cancellationToken)
    {
        try
        {
            await EnsureChannelAsync(cancellationToken);
            var message = JsonSerializer.Serialize(wrapper);
            var body = Encoding.UTF8.GetBytes(message);

            await _channel!.BasicPublishAsync(exchange: options.ExchangeName,
                routingKey: string.Empty,
                mandatory: false,
                basicProperties: new BasicProperties
                {
                    ContentType = "text/plain",
                    DeliveryMode = DeliveryModes.Persistent
                }
                , body: body
                , cancellationToken: cancellationToken);

            logger.LogInformation("Event published to RabbitMQ: {EventType}", wrapper.EventType);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to publish event to RabbitMQ: {EventType}", wrapper.EventType);
            throw;
        }
    }
}