using Hoyoverse.Infrastructure.Common;
using MongoDB.Bson;

namespace Hoyoverse.Infrastructure.Repositories;

public interface IRepository<TEntity, TKey> : IBulkRepository<TEntity>, IDisposable where TEntity : AuditableEntity<TKey>, new()
{
    Task<TEntity> InsertAsync(TEntity value);
    Task<TEntity> FindByIdAsync(TKey id);
    Task<IEnumerable<TEntity>> GetAll();
    Task<TEntity> UpdateAsync(TEntity value);
    Task<bool> DeleteAsync(TKey id);
}