namespace Hoyoverse.Infrastructure.Repositories;

public interface IBulkRepository<TEntity>
{
    Task BulkInsertAsync(IEnumerable<TEntity> items);
    Task BulkUpdateAsync(IEnumerable<TEntity> items);
    Task BulkDeleteAsync(IEnumerable<TEntity> items);
}
