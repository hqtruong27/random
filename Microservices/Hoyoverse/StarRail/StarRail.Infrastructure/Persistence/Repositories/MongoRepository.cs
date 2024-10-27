﻿using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using StarRail.Core.Base;
using StarRail.Core.Interfaces.Repositories;
using System.Linq.Expressions;

namespace StarRail.Infrastructure.Persistence.Repositories;

public class MongoRepository<TEntity, TKey>(IStarRailDbContext context)
    : IRepository<TEntity, TKey> where TEntity
    : AuditableEntity<TKey>, new()
{
    public IQueryable<TEntity> Queries => _collection.AsQueryable();

    private IMongoCollection<TEntity> _collection => context.Set<TEntity>();

    public async Task<IEnumerable<BsonDocument>> AggregateAsync(params BsonDocument[] pipeline)
    {
        var result = await context.Set<TEntity>().AggregateAsync<BsonDocument>(pipeline);
        return await result.ToListAsync();
    }

    public async Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var total = await _collection.CountDocumentsAsync(predicate);
        return total;
    }

    public async Task<TEntity> FindByIdAsync(TKey id)
    {
        var buildQuery = FilterId(id);
        var cursor = await _collection.FindAsync(buildQuery);

        return await cursor.SingleOrDefaultAsync();
    }

    public async Task<IEnumerable<TEntity>> FindByConditionsAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var buildQuery = _collection.AsQueryable();
        var result = await buildQuery.Where(predicate).ToListAsync();
        return result;
    }

    public async Task<IAsyncCursor<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _collection.FindAsync(predicate);
    }

    public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var items = await FindAsync(predicate);

        return await items.FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<TEntity>> GetAll()
    {
        var buildQuery = Builders<TEntity>.Filter.Empty;
        var result = await _collection.FindAsync(buildQuery);

        return await result.ToListAsync();
    }

    public async Task<TEntity> InsertAsync(TEntity entity)
    {
        entity.Created = DateTime.UtcNow;
        await _collection.InsertOneAsync(entity);

        return entity;
    }

    public async Task<bool> DeleteAsync(TKey id)
    {
        var result = await _collection.DeleteOneAsync(FilterId(id));

        return result.IsAcknowledged;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        entity.Updated = DateTime.UtcNow;
        await _collection.ReplaceOneAsync(FilterId(entity.Id), entity);

        return entity;
    }

    public Task BulkInsertAsync(IEnumerable<TEntity> items)
    {
        var documents = items.ToList();
        documents.ForEach(x => x.Created = DateTime.UtcNow);

        return _collection.InsertManyAsync(documents);
    }

    public Task BulkUpdateAsync(IEnumerable<TEntity> items)
    {
        throw new NotImplementedException();
    }

    public Task BulkDeleteAsync(IEnumerable<TEntity> items)
    {
        var ids = items.Select(x => x.Id).ToList();
        var filter = Builders<TEntity>.Filter.Where(x => ids.Contains(x.Id));

        return _collection.DeleteManyAsync(filter);
    }

    private static FilterDefinition<TEntity> FilterId(TKey key)
    {
        return Builders<TEntity>.Filter.Eq(x => x.Id, key);
    }

    public void Dispose() => GC.SuppressFinalize(this);
}
