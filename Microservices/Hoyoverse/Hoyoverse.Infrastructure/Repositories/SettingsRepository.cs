using Hoyoverse.Infrastructure.Entities;
using MongoDB.Bson.Serialization;

namespace Hoyoverse.Infrastructure.Repositories;

public class SettingRepository(IRepository<Setting, string> repository) : ISettingRepository
{
    private readonly IRepository<Setting, string> _repository = repository;

    public async Task<T> Read<T>(string key)
    {
        var setting = await _repository.FirstOrDefaultAsync(x => x.Key == key);

        return BsonSerializer.Deserialize<T>(setting.Value);
    }
}
