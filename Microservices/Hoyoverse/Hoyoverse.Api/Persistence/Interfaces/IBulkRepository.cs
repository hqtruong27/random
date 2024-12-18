namespace Hoyoverse.Persistence.Interfaces;

public interface IBulkRepository<in TEntity>
{
    Task BulkInsertAsync(IEnumerable<TEntity> items, CancellationToken token = default);
    Task BulkUpdateAsync(IEnumerable<TEntity> items, CancellationToken token = default);
    Task BulkDeleteAsync(IEnumerable<TEntity> items, CancellationToken token = default);
}
