namespace Infrastructure.Persistence;

public interface IEventStore
{
    Task SaveEventAsync<T>(Guid aggregateId, T eventData, DateTime timestamp, string eventType);
    Task<List<object>> GetEventsAsync(Guid aggregateId);
}
