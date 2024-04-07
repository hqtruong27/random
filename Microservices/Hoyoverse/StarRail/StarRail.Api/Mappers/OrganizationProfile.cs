using AutoMapper;
using Random.ExternalService.Models.Response;
using StarRail.Api.Features.Models;
using StarRail.Core.Entities;
using StarRail.Core.Enums;
using System.Globalization;
using System.Linq.Expressions;

namespace StarRail.Api.Mappers;

public class OrganizationProfile : Profile
{
    public OrganizationProfile()
    {
        CreateMap<GachaHistoryItemResponse, GachaHistory>()
           .Map(x => x.Time, r => DateTime.ParseExact(r.Time
               , "yyyy-MM-dd HH:mm:ss"
               , CultureInfo.InvariantCulture
               , DateTimeStyles.None))
           .Map(x => x.ItemType, r => Convert(r.ItemType, r.Lang))
           .Map(x => x.ReferenceId, r => r.Id)
           .Ignore(x => x.Id)
           .ReverseMap();

        CreateMap<Event, WishHistoryEvent>()
            .ReverseMap();

        CreateMap<WishBanner, WishHistoryDetail>()
            .ReverseMap();

        CreateMap<WishCounterModel, WishHistory>()
            .Map(x => x.Banner, i => i.Banner.ToString())
            .ReverseMap();
    }

    private static ItemType Convert(string itemType, string lang)
    {
        return lang switch
        {
            "vi-vn" => itemType == "Nón Ánh Sáng" ? ItemType.LightCone : ItemType.Character,
            "en-us" => itemType == "Light Cone" ? ItemType.LightCone : ItemType.Character,
            _ => ItemType.Character,
        };
    }
}

public static class MapperExtensions
{
    public static IMappingExpression<TSource, TDestination> Map<TSource, TMember, TDestination>(
        this IMappingExpression<TSource, TDestination> mappingExpression,
        Expression<Func<TDestination, TMember>> destinationMember,
        Expression<Func<TSource, TMember>> sourceMember) where TDestination : class
    => mappingExpression.ForMember(destinationMember, x => x.MapFrom(sourceMember));

    public static IMappingExpression<TSource, TDestination> Ignore<TSource, TMember, TDestination>(
        this IMappingExpression<TSource, TDestination> mappingExpression,
        Expression<Func<TDestination, TMember>> destinationMember) where TDestination : class
    => mappingExpression.ForMember(destinationMember, x => x.Ignore());
}

