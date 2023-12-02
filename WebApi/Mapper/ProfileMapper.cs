﻿using Common.Enum;
using System.Linq.Expressions;

namespace WebApi.Mapper;

public class OrganizationProfile : Profile
{
    public OrganizationProfile()
    {
        CreateMap<Spending, GetSpendingResponse>();
        CreateMap<CreateSpendingRequest, Spending>()
            .Map(a => a.Status, x => Enum.Parse<SpendingStatus>(x.Status))
            .Map(a => a.Purpose, x => Enum.Parse<SpendingPurpose>(x.Purpose));
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
