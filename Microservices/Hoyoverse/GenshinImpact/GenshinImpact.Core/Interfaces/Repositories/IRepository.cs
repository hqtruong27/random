using System.Linq.Expressions;
using GenshinImpact.Core.Base;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GenshinImpact.Core.Interfaces.Repositories;

public interface IRepository<TEntity, in TKey> : IBulkRepository<TEntity>, IDisposable where TEntity : IAuditableEntity<TKey>
{
    IQueryable<TEntity> Queries { get; }
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

public interface IRepository<TEntity> : IRepository<TEntity, string> where TEntity : IAuditableEntity;