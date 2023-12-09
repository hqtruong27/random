using GenshinImpact.Api.Settings;
using Hoyoverse.Infrastructure.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GenshinImpact.Api.Services;

public class MongoDbService
{
    private readonly IMongoCollection<GachaHistory> _gachaHistory;
    public MongoDbService(IOptions<MongoDbSettings> options)
    {
        var client = new MongoClient(MongoUrl.Create(options.Value.ConnectionString));
        var database = client.GetDatabase(options.Value.Database);
        _gachaHistory = database.GetCollection<GachaHistory>(options.Value.ConnectionName);
    }

    public async Task InsertAsync(GachaHistory item)
    {
        await _gachaHistory.InsertOneAsync(item);
    }

    public async Task BulkInsertAsync(IEnumerable<GachaHistory> items)
    {
        await _gachaHistory.InsertManyAsync(items);
    }
}