namespace GenshinImpact.Core.Interfaces.Repositories;

public interface IBulkRepository<in TEntity>
{
    Task BulkInsertAsync(IEnumerable<TEntity> items);
    Task BulkUpdateAsync(IEnumerable<TEntity> items);
    Task BulkDeleteAsync(IEnumerable<TEntity> items);
}
