using GenshinImpact.Core.EnumTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.EntityFrameworkCore.Extensions;

namespace GenshinImpact.Infrastructure.Persistence.Configurations;

public class GachaHistoryConfiguration : IEntityTypeConfiguration<GachaHistory>
{
    public void Configure(EntityTypeBuilder<GachaHistory> builder)
    {
        builder.ToCollection("GachaHistories");

        //builder.Property(x => x.Name).HasMaxLength(500);
        //builder.Property(x => x.Uid).HasMaxLength(250);
        //builder.Property(x => x.ItemId).HasMaxLength(250);
        //builder.Property(x => x.Lang).HasMaxLength(50);

        //builder
        //    .Property(x => x.ItemType)
        //    .HasConversion(
        //        value => value.ToString(),
        //        value => Enum.Parse<ItemType>(value)
        //        );

        //builder
        //    .Property(x => x.RankType)
        //    .HasConversion(
        //        value => value.ToString(),
        //        value => Enum.Parse<RankType>(value)
        //    );

        //builder
        //    .Property(x => x.GachaType)
        //    .HasConversion(
        //        value => value.ToString(),
        //        value => Enum.Parse<GachaType>(value)
        //    );
    }

    public static void Configure()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(GachaHistory)))
        {
            BsonClassMap.RegisterClassMap<GachaHistory>((classMap =>
            {
                classMap.AutoMap();
                classMap.MapMember(x => x.RankType).SetSerializer(new EnumSerializer<RankType>(BsonType.String));
                classMap.MapMember(x => x.GachaType).SetSerializer(new EnumSerializer<GachaType>(BsonType.String));
                classMap.MapMember(x => x.ItemType).SetSerializer(new EnumSerializer<ItemType>(BsonType.String));
            }));
        }
    }
}