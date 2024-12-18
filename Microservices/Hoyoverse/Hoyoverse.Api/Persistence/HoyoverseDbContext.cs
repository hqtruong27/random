namespace Hoyoverse.Persistence;

public class HoyoverseDbContext(IDbContextOptions options) : MongoDbContext(options)
{
    public IMongoCollection<User> Users 
        => Database.GetCollection<User>(nameof(User).Underscore());

    public IMongoCollection<Option> Options 
        => Database.GetCollection<Option>(nameof(Option).Underscore());

    public IMongoCollection<GenshinImpactGachaHistory> GenshinImpactGachaHistories 
        => Database.GetCollection<GenshinImpactGachaHistory>(nameof(GenshinImpactGachaHistory).Underscore());

    public IMongoCollection<StarRailGachaHistory> StarRailGachaHistories
       => Database.GetCollection<StarRailGachaHistory>(nameof(StarRailGachaHistory).Underscore());
}