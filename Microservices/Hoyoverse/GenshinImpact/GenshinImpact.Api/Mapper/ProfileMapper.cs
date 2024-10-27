using GenshinImpact.Shared.Converters;
using GenshinImpact.Shared.Extensions;
using System.Globalization;

namespace GenshinImpact.Api.Mapper;

public class OrganizationProfile : Profile
{
    public OrganizationProfile()
    {
        CreateMap<GachaHistoryResponse, GachaHistory>()
            .Map(x => x.Time, r => DateTimeOffset.ParseExact(r.Time
                , "yyyy-MM-dd HH:mm:ss"
                , CultureInfo.InvariantCulture
                , DateTimeStyles.None))
            .Map(x => x.ItemType, r => GachaItemTypeConverter.Convert(r.ItemType, r.Lang))
            .Map(x => x.ReferenceId, r => r.Id)
            .Ignore(x => x.Id)
            .ReverseMap();
    }
}