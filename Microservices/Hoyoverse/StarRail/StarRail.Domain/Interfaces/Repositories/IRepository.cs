using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using StarRail.Domain.Core;
using System.Linq.Expressions;

namespace StarRail.Domain.Interfaces.Repositories;

public interface IRepository<TEntity, TKey> : IBulkRepository<TEntity>, IDisposable where TEntity : AuditableEntity<TKey>
{
    public IMongoCollection<TEntity> Collection { get; }
    IMongoQueryable<TEntity> Queries { get; }
    Task<IEnumerable<BsonDocument>> AggregateAsync(params BsonDocument[] pipeline);
    Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity> InsertAsync(TEntity value);
    Task<TEntity> FindByIdAsync(TKey id);
    Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
    Task<IAsyncCursor<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
    Task<IEnumerable<TEntity>> FindByConditionsAsync(Expression<Func<TEntity, bool>> predicate);
    Task<IEnumerable<TEntity>> GetAll();
    Task<TEntity> UpdateAsync(TEntity value);
    Task<bool> DeleteAsync(TKey id);
}