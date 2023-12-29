using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Dispatcher.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection Register(this IServiceCollection services, Assembly assembly)
    {
        return services
            .AddRequiredServices()
            .Add(typeof(IRequestHandler<>), assembly)
            .Add(typeof(IRequestHandler<,>), assembly);
    }

    private static IServiceCollection Add(this IServiceCollection services, Type requestInterface, Assembly assembly)
    {
        List<Type> interfaces = [];
        List<Type> concretions = [];
        foreach (var type in assembly.DefinedTypes.Where(t => !t.IsOpenGeneric()).Where(t => true))
        {
            var interfaceTypes = type.FindInterfacesThatClose(requestInterface).ToArray();
            if (interfaceTypes.Length == 0) continue;

            if (type.IsConcrete())
            {
                concretions.Add(type);
            }

            foreach (var interfaceType in interfaceTypes)
            {
                interfaces.Fill(interfaceType);
            }
        }

        foreach (var @interface in interfaces)
        {
            var exactMatches = concretions.Where(x => x.CanBeCastTo(@interface)).ToList();
            foreach (var type in exactMatches)
            {
                services.AddTransient(@interface, type);
            }
        }

        return services;
    }

    private static IServiceCollection AddRequiredServices(this IServiceCollection services)
    {
        services.TryAddSingleton<IDispatcher, Dispatcher>();
        services.TryAdd(new ServiceDescriptor(typeof(ISender), x => x.GetRequiredService<IDispatcher>(), ServiceLifetime.Singleton));

        return services;
    }

    private static bool CanBeCastTo(this Type pluggedType, Type pluginType)
    {
        if (pluggedType == null) return false;

        if (pluggedType == pluginType) return true;

        return pluginType.IsAssignableFrom(pluggedType);
    }

    public static IEnumerable<Type> FindInterfacesThatClose(this Type pluggedType, Type templateType)
    {
        return FindInterfacesThatClosesCore(pluggedType, templateType).Distinct();
    }

    public static IEnumerable<Type> FindInterfacesThatClosesCore(Type pluggedType, Type templateType)
    {
        if (pluggedType == null) yield break;

        if (!pluggedType.IsConcrete()) yield break;

        if (templateType.IsInterface)
        {
            foreach (
                var interfaceType in
                pluggedType.GetInterfaces()
                    .Where(type => type.IsGenericType && type.GetGenericTypeDefinition() == templateType))
            {
                yield return interfaceType;
            }
        }
        else if (pluggedType.BaseType!.IsGenericType &&
                 pluggedType.BaseType!.GetGenericTypeDefinition() == templateType)
        {
            yield return pluggedType.BaseType!;
        }

        if (pluggedType.BaseType == typeof(object)) yield break;

        foreach (var interfaceType in FindInterfacesThatClosesCore(pluggedType.BaseType!, templateType))
        {
            yield return interfaceType;
        }
    }

    public static bool IsConcrete(this Type type)
    {
        return !type.IsAbstract && !type.IsInterface;
    }

    public static void Fill<T>(this IList<T> list, T value)
    {
        if (list.Contains(value)) return;
        list.Add(value);
    }

    public static bool IsOpenGeneric(this Type type)
    {
        return type.IsGenericTypeDefinition || type.ContainsGenericParameters;
    }
}
