namespace Hoyoverse.Infrastructure.Repositories;

public interface ISettingsRepository
{
    Task<T> GetSettingsAsync<T>(string key);
}
