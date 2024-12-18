namespace Infrastructure.Persistence;

public class KuroDbContext(IDbContextOptions options) : MongoDbContext(options)
{
    public IMongoCollection<ConveneRecord> ConveneRecords
        => Database.GetCollection<ConveneRecord>(nameof(ConveneRecord).Underscore());
}