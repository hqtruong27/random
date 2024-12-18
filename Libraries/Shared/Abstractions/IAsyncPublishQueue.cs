namespace Shared.Abstractions;

public interface IAsyncPublishQueue<T>
{
    ValueTask EnqueueAsync(T item);
    void StartProcessing(CancellationToken cancellationToken = default);
}
