namespace Hoyoverse.Persistence.Configurations;

public class StarRailGachaHistoryConfiguration : ISchemaConfiguration
{
    public void Configure()
    {
        BsonClassMap.RegisterClassMap<StarRailGachaHistory>(map =>
        {
            map.AutoMap();
            map.MapMember(x => x.RankType)
            .SetSerializer(new EnumSerializer<StarRailRankType>(BsonType.String));

            map.MapMember(x => x.GachaType)
            .SetSerializer(new EnumSerializer<StarRailGachaType>(BsonType.String));

            map.MapMember(x => x.ItemType)
            .SetSerializer(new EnumSerializer<StarRailItemType>(BsonType.String));
        });
    }
}
