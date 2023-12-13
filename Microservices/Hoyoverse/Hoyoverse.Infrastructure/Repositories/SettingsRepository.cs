using Hoyoverse.Infrastructure.Entities;
using MongoDB.Bson.Serialization;

namespace Hoyoverse.Infrastructure.Repositories;

public class SettingsRepository(IRepository<Settings, string> repository) : ISettingsRepository
{
    private readonly IRepository<Settings, string> _repository = repository;

    public async Task<T> GetSettingsAsync<T>(string key)
    {
        var setting = await _repository.FirstOrDefaultAsync(x => x.Key == key);

        return BsonSerializer.Deserialize<T>(setting.Value);
    }
}
