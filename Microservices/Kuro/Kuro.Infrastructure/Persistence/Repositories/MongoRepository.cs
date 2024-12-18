namespace Infrastructure.Persistence.Repositories;

public class MongoRepository<TEntity, TKey>(KuroDbContext context)
    : IRepository<TEntity, TKey> where TEntity
    : IAuditableEntity<TKey>, new()
{
    public IQueryable<TEntity> Queries => _collection.AsQueryable();

    private IMongoCollection<TEntity> _collection => context.Set<TEntity>();

    public async Task<IEnumerable<BsonDocument>> AggregateAsync(params BsonDocument[] pipeline)
    {
        var result = await context.Set<TEntity>().AggregateAsync<BsonDocument>(pipeline);
        return await result.ToListAsync();
    }

    public async Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken token = default)
    {
        var total = await _collection.CountDocumentsAsync(predicate, cancellationToken: token);
        return total;
    }

    public async Task<TEntity> FindByIdAsync(TKey id, CancellationToken token = default)
    {
        var buildQuery = FilterId(id);
        var cursor = await _collection.FindAsync(buildQuery, cancellationToken: token);

        return await cursor.SingleOrDefaultAsync(token);
    }

    public async Task<IEnumerable<TEntity>> FindByConditionsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken token = default)
    {
        var buildQuery = _collection.AsQueryable();
        var result = await buildQuery.Where(predicate).ToListAsync(token);
        return result;
    }

    public async Task<IAsyncCursor<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken token = default)
    {
        return await _collection.FindAsync(predicate, cancellationToken: token);
    }

    public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken token = default)
    {
        var items = await FindAsync(predicate, token);

        return await items.FirstOrDefaultAsync(token);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken token = default)
    {
        var buildQuery = Builders<TEntity>.Filter.Empty;
        var result = await _collection.FindAsync(buildQuery, cancellationToken: token);

        return await result.ToListAsync(token);
    }

    public async Task<TEntity> InsertAsync(TEntity entity, CancellationToken token = default)
    {
        entity.Created = DateTime.UtcNow;
        await _collection.InsertOneAsync(entity, cancellationToken: token);

        return entity;
    }

    public async Task<bool> DeleteAsync(TKey id, CancellationToken token = default)
    {
        var result = await _collection.DeleteOneAsync(FilterId(id), token);

        return result.IsAcknowledged;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken token = default)
    {
        entity.Updated = DateTime.UtcNow;
        await _collection.ReplaceOneAsync(FilterId(entity.Id), entity, cancellationToken: token);

        return entity;
    }

    public Task BulkInsertAsync(IEnumerable<TEntity> items, CancellationToken token = default)
    {
        var documents = items.ToList();
        documents.ForEach(x => x.Created = DateTime.UtcNow);

        return _collection.InsertManyAsync(documents, cancellationToken: token);
    }

    public Task BulkUpdateAsync(IEnumerable<TEntity> items, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task BulkDeleteAsync(IEnumerable<TEntity> items, CancellationToken token = default)
    {
        var ids = items.Select(x => x.Id).ToList();
        var filter = Builders<TEntity>.Filter.Where(x => ids.Contains(x.Id));

        return _collection.DeleteManyAsync(filter, token);
    }

    private static FilterDefinition<TEntity> FilterId(TKey key)
    {
        return Builders<TEntity>.Filter.Eq(x => x.Id, key);
    }

    public void Dispose() => GC.SuppressFinalize(this);
}

public class MongoRepository<TEntity>(KuroDbContext context)
    : MongoRepository<TEntity, string>(context), IRepository<TEntity> where TEntity
    : IAuditableEntity, new();