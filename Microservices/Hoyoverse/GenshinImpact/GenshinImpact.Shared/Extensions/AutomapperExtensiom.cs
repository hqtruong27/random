using AutoMapper;
using System.Linq.Expressions;

namespace GenshinImpact.Shared.Extensions;

public static class AutomapperExtensions
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