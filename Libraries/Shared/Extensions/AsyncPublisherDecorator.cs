using System.Threading.Channels;
using Microsoft.Extensions.Logging;

namespace Shared.Extensions;

public class AsyncPublisherDecorator(IServiceProvider serviceProvider) : IPublisher
{
    public async Task Publish(object notification, CancellationToken cancellationToken = default)
    {
        await PublishQueue<object>().EnqueueAsync(notification);
    }

    public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        await PublishQueue<TNotification>().EnqueueAsync(notification);
    }

    private IAsyncPublishQueue<T> PublishQueue<T>()
    {
        return serviceProvider.GetService<IAsyncPublishQueue<T>>()
            ?? throw new InvalidOperationException(
                $"No AsyncPublishQueue registered"
                );
    }
}

public class AsyncPublishQueue<T> : IAsyncPublishQueue<T>
{
    private readonly IMediator _mediator;
    private readonly Channel<T> _channel;
    private readonly ILogger<AsyncPublishQueue<T>> _logger;
    private const int MaxRetryAttempts = 3;
    private const int BaseDelayMs = 1000;

    public AsyncPublishQueue(IMediator mediator, ILogger<AsyncPublishQueue<T>> logger)
    {
        _mediator = mediator;
        _logger = logger;
        _channel = Channel.CreateUnbounded<T>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });

        StartProcessing();
    }

    public ValueTask EnqueueAsync(T item)
    {
        return _channel.Writer.WriteAsync(item);
    }

    public void StartProcessing(CancellationToken cancellationToken = default)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await foreach (var item in _channel.Reader.ReadAllAsync(cancellationToken))
                {
                    await ProcessWithRetryAsync(item, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Processing canceled.");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Critical error in processing loop.");
            }
        }, cancellationToken);
    }

    private async Task ProcessWithRetryAsync(T item, CancellationToken cancellationToken)
    {
        int attempt = 0;
        while (true)
        {
            try
            {
                attempt++;
                await _mediator.Publish(notification: item!, cancellationToken);
                return;
            }
            catch (Exception ex) when (attempt <= MaxRetryAttempts)
            {
                var delay = CalculateDelay(attempt);
                _logger.LogWarning(
                    ex,
                    "Error processing item. Retrying in {Delay}ms (Attempt {Attempt}/{MaxAttempts}).",
                    delay,
                    attempt,
                    MaxRetryAttempts
                    );

                await Task.Delay(delay, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to process item after {MaxAttempts} attempts.",
                    MaxRetryAttempts
                    );

                return;
            }
        }
    }

    private static int CalculateDelay(int attempt)
    {
        // Exponential back off with jitter (randomness to avoid collisions)
        var random = new Random();
        return BaseDelayMs * (int)Math.Pow(2, attempt - 1) + random.Next(0, 100);
    }
}
