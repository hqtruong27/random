using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq.Expressions;

namespace Hoyoverse.Infrastructure.Repositories;

public interface IRepository<TEntity, TKey> : IBulkRepository<TEntity>, IDisposable where TEntity : AuditableEntity<TKey>
{
    IMongoQueryable<TEntity> Queries { get; }
    Task<IEnumerable<BsonDocument>> AggregateAsync(params BsonDocument[] pipeline);
    Task<long> CountAsync(Expression<Func<TEntity, bool>> whereConditions);
    Task<TEntity> InsertAsync(TEntity value);
    Task<TEntity> FindByIdAsync(TKey id);
    Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
    Task<IAsyncCursor<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
    Task<IEnumerable<TEntity>> FindByConditionsAsync(Expression<Func<TEntity, bool>> whereConditions);
    Task<IEnumerable<TEntity>> GetAll();
    Task<TEntity> UpdateAsync(TEntity value);
    Task<bool> DeleteAsync(TKey id);
}