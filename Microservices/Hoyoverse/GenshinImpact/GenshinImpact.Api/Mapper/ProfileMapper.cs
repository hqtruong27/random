﻿using AutoMapper;
using Common.Enum.Hoyoverse;
using Hoyoverse.Infrastructure.Entities;
using Models.GenshinImpact;
using System.Globalization;
using System.Linq.Expressions;

namespace GenshinImpact.Api.Mapper;

public class OrganizationProfile : Profile
{
    public OrganizationProfile()
    {
        CreateMap<GachaHistoryResponse, GachaHistory>()
            .Map(x => x.Time, r => DateTime.ParseExact(r.Time
                , "yyyy-MM-dd HH:mm:ss"
                , CultureInfo.InvariantCulture
                , DateTimeStyles.None))
            .Map(x => x.ItemType, r => r.ItemType == "Vũ Khí" ? ItemType.Weapons : ItemType.Character);
    }
}

public static class MapperExtensions
{
    public static IMappingExpression<TSource, TDestination> Map<TSource, TMember, TDestination>(
        this IMappingExpression<TSource, TDestination> mappingExpression,
        Expression<Func<TDestination, TMember>> destinationMember,
        Expression<Func<TSource, TMember>> sourceMember) where TDestination : class
    => mappingExpression.ForMember(destinationMember, x => x.MapFrom(sourceMember));
}
