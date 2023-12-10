﻿using Hoyoverse.Infrastructure;
using Hoyoverse.Infrastructure.Common;
using Hoyoverse.Infrastructure.Repositories;
using MongoDB.Driver;

namespace GenshinImpact.Persistence.Repositories;

public class MongoRepository<TEntity, TKey>(IHoyoverseDbContext context) 
    : IRepository<TEntity, TKey> where TEntity 
    : AuditableEntity<TKey>, new()
{
    private readonly IMongoCollection<TEntity> _collection = context.Database.GetCollection<TEntity>(typeof(TEntity).Name);

    public async Task<TEntity> FindByIdAsync(TKey id)
    {
        var buildQuery = FilterId(id);
        var cursor = await _collection.FindAsync(buildQuery);

        return await cursor.SingleOrDefaultAsync();
    }

    public async Task<IEnumerable<TEntity>> GetAll()
    {
        var buildQuery = Builders<TEntity>.Filter.Empty;
        var result = await _collection.FindAsync(buildQuery);

        return await result.ToListAsync();
    }

    public async Task<TEntity> InsertAsync(TEntity entity)
    {
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
        await _collection.ReplaceOneAsync(FilterId(entity.Id), entity);

        return entity;
    }

    public Task BulkInsertAsync(IEnumerable<TEntity> items)
    {
        return _collection.InsertManyAsync(items);
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