namespace Hoyoverse.Infrastructure.Repositories;

public interface ISettingRepository
{
    Task<T> Read<T>(string key);
}
