using System.Globalization;
using Hoyoverse.Shared.Extensions;

namespace Hoyoverse.Mapper;

public class OrganizationProfile : Profile
{
    public OrganizationProfile()
    {
        CreateMap<GachaHistoryItemResponse, GenshinImpactGachaHistory>()
            .Map(x => x.Time, r => DateTimeOffset.ParseExact(r.Time
                , "yyyy-MM-dd HH:mm:ss"
                , CultureInfo.InvariantCulture
                , DateTimeStyles.None))
            .Map(x => x.ItemType, r => Convert(r.ItemType, r.Lang))
            .Map(x => x.ReferenceId, r => r.Id)
            .Ignore(x => x.Id)
            .ReverseMap();

        CreateMap<GachaHistoryItemResponse, StarRailGachaHistory>()
            .Map(x => x.Time, r => DateTimeOffset.ParseExact(r.Time
                , "yyyy-MM-dd HH:mm:ss"
                , CultureInfo.InvariantCulture
                , DateTimeStyles.None))
            .Map(x => x.ItemType, r => HsrConvert(r.ItemType, r.Lang))
            .Map(x => x.ReferenceId, r => r.Id)
            .Ignore(x => x.Id)
            .ReverseMap();
    }

    public static GenshinImpactItemType Convert(string itemType, string lang)
    {
        return lang switch
        {
            "vi-vn" => itemType.Equals(
                NameTranslator.Weapons,
                StringComparison.CurrentCultureIgnoreCase
                )
            ? GenshinImpactItemType.Weapons
            : GenshinImpactItemType.Character,
            "en-us" => Enum.Parse<GenshinImpactItemType>(itemType),
            _ => Enum.Parse<GenshinImpactItemType>(itemType)
        };
    }

    public static StarRailItemType HsrConvert(string itemType, string lang)
    {
        return lang switch
        {
            "vi-vn" => itemType.Equals(
                NameTranslator.LightCone,
                StringComparison.CurrentCultureIgnoreCase
                )
            ? StarRailItemType.LightCone
            : StarRailItemType.Character,
            "en-us" => Enum.Parse<StarRailItemType>(itemType),
            _ => Enum.Parse<StarRailItemType>(itemType)
        };
    }
}