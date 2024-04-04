using MongoDB.Driver;
using System.Linq.Expressions;
using MongoDB.Driver.Linq;
using MongoDB.Bson;
using StarRail.Core.Interfaces.Repositories;
using StarRail.Core.Base;

namespace StarRail.Infrastructure.Persistence.Repositories;

public class MongoRepository<TEntity, TKey>(IStarRailDbContext context)
    : IRepository<TEntity, TKey> where TEntity
    : AuditableEntity<TKey>, new()
{
    private readonly IMongoCollection<TEntity> _collection = context.Set<TEntity>();

    public IMongoCollection<TEntity> Collection => _collection;

    public IMongoQueryable<TEntity> Queries => context.Set<TEntity>().AsQueryable();

    public async Task<IEnumerable<BsonDocument>> AggregateAsync(params BsonDocument[] pipeline)
    {
        var result = await context.Set<TEntity>().AggregateAsync<BsonDocument>(pipeline);
        return await result.ToListAsync();
    }

    public async Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var total = await Collection.CountDocumentsAsync(predicate);
        return total;
    }

    public async Task<TEntity> FindByIdAsync(TKey id)
    {
        var buildQuery = FilterId(id);
        var cursor = await Collection.FindAsync(buildQuery);

        return await cursor.SingleOrDefaultAsync();
    }

    public async Task<IEnumerable<TEntity>> FindByConditionsAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var buildQuery = Collection.AsQueryable();
        var result = await buildQuery.Where(predicate).ToListAsync();
        return result;
    }

    public async Task<IAsyncCursor<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await Collection.FindAsync(predicate);
    }

    public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var items = await FindAsync(predicate);

        return await items.FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<TEntity>> GetAll()
    {
        var buildQuery = Builders<TEntity>.Filter.Empty;
        var result = await Collection.FindAsync(buildQuery);

        return await result.ToListAsync();
    }

    public async Task<TEntity> InsertAsync(TEntity entity)
    {
        entity.Created = DateTime.UtcNow;
        await Collection.InsertOneAsync(entity);

        return entity;
    }

    public async Task<bool> DeleteAsync(TKey id)
    {
        var result = await Collection.DeleteOneAsync(FilterId(id));

        return result.IsAcknowledged;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        entity.Updated = DateTime.UtcNow;
        await Collection.ReplaceOneAsync(FilterId(entity.Id), entity);

        return entity;
    }

    public Task BulkInsertAsync(IEnumerable<TEntity> items)
    {
        items.ToList().ForEach(x => x.Created = DateTime.UtcNow);

        return Collection.InsertManyAsync(items);
    }

    public Task BulkUpdateAsync(IEnumerable<TEntity> items)
    {
        throw new NotImplementedException();
    }

    public Task BulkDeleteAsync(IEnumerable<TEntity> items)
    {
        var ids = items.Select(x => x.Id).ToList();
        var filter = Builders<TEntity>.Filter.Where(x => ids.Contains(x.Id));

        return Collection.DeleteManyAsync(filter);
    }

    private static FilterDefinition<TEntity> FilterId(TKey key)
    {
        return Builders<TEntity>.Filter.Eq(x => x.Id, key);
    }

    public void Dispose() => GC.SuppressFinalize(this);
}
