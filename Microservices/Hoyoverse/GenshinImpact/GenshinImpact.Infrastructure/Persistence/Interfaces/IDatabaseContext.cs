namespace GenshinImpact.Infrastructure.Persistence.Interfaces;

public interface IDatabaseContext
{
    IMongoCollection<GachaHistory> GachaHistories { get; }
    IMongoCollection<T> Collection<T>();
    IMongoCollection<T> Collection<T>(string name);
}