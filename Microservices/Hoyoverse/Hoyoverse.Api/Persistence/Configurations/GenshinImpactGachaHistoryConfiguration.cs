namespace Hoyoverse.Persistence.Configurations;

public class GenshinImpactGachaHistoryConfiguration : ISchemaConfiguration
{
    public void Configure()
    {
        BsonClassMap.RegisterClassMap<GenshinImpactGachaHistory>(map =>
        {
            map.AutoMap();
            map.MapMember(x => x.RankType)
            .SetSerializer(new EnumSerializer<GenshinImpactRankType>(BsonType.String));

            map.MapMember(x => x.GachaType)
            .SetSerializer(new EnumSerializer<GenshinImpactGachaType>(BsonType.String));

            map.MapMember(x => x.ItemType)
            .SetSerializer(new EnumSerializer<GenshinImpactItemType>(BsonType.String));
        });
    }
}
