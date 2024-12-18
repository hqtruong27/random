namespace Infrastructure.Persistence.Extensions;

public static class MongoDbExtensions
{
    public static Task BulkInsertAsync<TDocument>(
        this IMongoCollection<TDocument> collection,
        IEnumerable<TDocument> items,
        CancellationToken token = default) where TDocument : IAuditableEntity, new()
    {
        var documents = items.ToList();
        documents.ForEach(x => x.Created = DateTime.UtcNow);

        return collection.InsertManyAsync(documents, cancellationToken: token);
    }

    public static Task BulkUpdateAsync<TDocument>(
        this IMongoCollection<TDocument> collection,
        IEnumerable<TDocument> items,
        CancellationToken token = default) where TDocument : IAuditableEntity, new()
    {
        return Task.CompletedTask;
    }

    public static Task BulkDeleteAsync<TDocument>(
        this IMongoCollection<TDocument> collection,
        IEnumerable<TDocument> items,
        CancellationToken token = default) where TDocument : IAuditableEntity, new()
    {
        var ids = items.Select(x => x.Id).ToList();
        var filter = Builders<TDocument>.Filter.Where(x => ids.Contains(x.Id));

        return collection.DeleteManyAsync(filter, token);
    }
}