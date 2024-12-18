namespace Infrastructure.Persistence.Repositories.Interfaces;

public interface IRepository<TEntity, in TKey> : IBulkRepository<TEntity>, IDisposable where TEntity : IAuditableEntity<TKey>
{
    IQueryable<TEntity> Queries { get; }
    Task<IEnumerable<BsonDocument>> AggregateAsync(params BsonDocument[] pipeline);
    Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken token = default);
    Task<TEntity> InsertAsync(TEntity value, CancellationToken token = default);
    Task<TEntity> FindByIdAsync(TKey id, CancellationToken token = default);
    Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken token = default);
    Task<IAsyncCursor<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken token = default);
    Task<IEnumerable<TEntity>> FindByConditionsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken token = default);
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken token = default);
    Task<TEntity> UpdateAsync(TEntity value, CancellationToken token = default);
    Task<bool> DeleteAsync(TKey id, CancellationToken token = default);
}

public interface IRepository<TEntity> : IRepository<TEntity, string> where TEntity : IAuditableEntity;