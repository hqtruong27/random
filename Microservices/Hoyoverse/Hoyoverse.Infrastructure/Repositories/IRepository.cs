using Hoyoverse.Infrastructure.Common;
using MongoDB.Bson;
using System.Linq.Expressions;

namespace Hoyoverse.Infrastructure.Repositories;

public interface IRepository<TEntity, TKey> : IBulkRepository<TEntity>, IDisposable where TEntity : AuditableEntity<TKey>, new()
{
    Task<IEnumerable<BsonDocument>> AggregateAsync(params BsonDocument[] pipeline);
    Task<long> CountAsync(Expression<Func<TEntity, bool>> whereConditions);
    Task<TEntity> InsertAsync(TEntity value);
    Task<TEntity> FindByIdAsync(TKey id);
    Task<IEnumerable<TEntity>> FindByConditionsAsync(Expression<Func<TEntity, bool>> whereConditions);
    Task<IEnumerable<TEntity>> GetAll();
    Task<TEntity> UpdateAsync(TEntity value);
    Task<bool> DeleteAsync(TKey id);
}