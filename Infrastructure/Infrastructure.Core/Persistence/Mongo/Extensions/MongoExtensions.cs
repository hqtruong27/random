using Infrastructure.Persistence.Mongo.Schemas;
using MongoDB.Driver;

namespace MongoDB.Driver;

public static class MongoExtensions
{
    public static Task BulkInsertAsync<TDocument>(
        this IMongoCollection<TDocument> collection,
        IEnumerable<TDocument> items,
        CancellationToken token = default) where TDocument : IAuditableEntity
    {
        foreach (var item in items)
        {
            item.Created = DateTime.UtcNow;
        }

        return collection.InsertManyAsync(items, cancellationToken: token);
    }

    public static async Task BulkUpdateAsync<TDocument>(
    this IMongoCollection<TDocument> collection,
    IEnumerable<TDocument> items,
    CancellationToken token = default) where TDocument : IAuditableEntity
    {
        var updates = new List<WriteModel<TDocument>>();
        foreach (var item in items)
        {
            item.Updated = DateTime.UtcNow;
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, item.Id);
            var update = Builders<TDocument>.Update.Set(doc => doc.Updated, item.Updated);
            updates.Add(new ReplaceOneModel<TDocument>(filter, item) { IsUpsert = false });
        }

        if (updates.Count != 0)
        {
            await collection.BulkWriteAsync(updates, new BulkWriteOptions { IsOrdered = false }, token);
        }
    }

    public static Task BulkDeleteAsync<TDocument>(
        this IMongoCollection<TDocument> collection,
        IEnumerable<TDocument> items,
        CancellationToken token = default) where TDocument : IAuditableEntity
    {
        var ids = items.Select(x => x.Id).ToList();
        var filter = Builders<TDocument>.Filter.Where(x => ids.Contains(x.Id));

        return collection.DeleteManyAsync(filter, token);
    }
}
