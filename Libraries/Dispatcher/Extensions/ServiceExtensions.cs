using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace Dispatcher.Extensions;

public static class ServiceCollectionExtensions
{
    public class DispatcherOptions
    {
        public bool AutoScanReferencedAssemblies { get; set; }
    }

    public static IServiceCollection AddDispatcher(this IServiceCollection services, Assembly assembly, Action<DispatcherOptions>? configure = default)
    {
        services.ScanInterface(typeof(IRequestHandler<>), assembly)
                .ScanInterface(typeof(IRequestHandler<,>), assembly)
                .AddRequiredServices();

        DispatcherOptions _configure = new();
        configure?.Invoke(_configure);

        if (_configure.AutoScanReferencedAssemblies)
        {
            services.AutoScanReferencedAssemblies(assembly);
        }

        return services;
    }

    private static IServiceCollection AutoScanReferencedAssemblies(this IServiceCollection services, Assembly assembly)
    {
        foreach (var assemblyName in assembly.GetReferencedAssemblies())
        {
            services.AddDispatcher(Assembly.Load(assemblyName));
        }

        return services;
    }

    private static IServiceCollection ScanInterface(this IServiceCollection services, Type requestInterface, Assembly assembly)
    {
        List<Type> interfaces = [];
        List<Type> concretions = [];
        foreach (var type in assembly.DefinedTypes.Where(t => !t.IsOpenGeneric()))
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
            if (exactMatches.Count > 1)
            {
                exactMatches.RemoveAll(x => !IsMatchingWithInterface(x, @interface));
            }

            foreach (var type in exactMatches)
            {
                services.TryAddTransient(@interface, type);
            }

            if (!@interface.IsOpenGeneric())
            {
                AddConcretions(@interface, concretions, services);
            }
        }

        return services;
    }

    private static bool IsMatchingWithInterface(Type? handlerType, Type handlerInterface)
    {
        if (handlerType == null || handlerInterface == null)
        {
            return false;
        }

        if (handlerType.IsInterface)
        {
            if (handlerType.GenericTypeArguments.SequenceEqual(handlerInterface.GenericTypeArguments))
            {
                return true;
            }
        }
        else
        {
            return IsMatchingWithInterface(handlerType.GetInterface(handlerInterface.Name), handlerInterface);
        }

        return false;
    }

    private static void AddConcretions(Type @interface, List<Type> concretions, IServiceCollection services)
    {
        foreach (var type in concretions.Where(x => x.IsOpenGeneric() && x.CouldCloseTo(@interface)))
        {
            try
            {
                services.TryAddTransient(@interface, type.MakeGenericType(@interface.GenericTypeArguments));
            }
            catch
            {
                //ignore
            }
        }
    }

    private static bool CouldCloseTo(this Type openConcretion, Type closedInterface)
    {
        var openInterface = closedInterface.GetGenericTypeDefinition();
        var arguments = closedInterface.GenericTypeArguments;

        var concreteArguments = openConcretion.GenericTypeArguments;
        return arguments.Length == concreteArguments.Length && openConcretion.CanBeCastTo(openInterface);
    }

    private static IServiceCollection AddRequiredServices(this IServiceCollection services)
    {
        //services.TryAddScoped(typeof(IRequestHandler<>), typeof(RequestHandlerWrapperImplement<>));
        //services.TryAddScoped(typeof(IRequestHandler<,>), typeof(RequestHandlerWrapperImplement<,>));
        services.TryAddTransient<IDispatcher, Dispatcher>();
        services.TryAddTransient(typeof(ISender), x => x.GetRequiredService<IDispatcher>());

        return services;
    }

    private static bool CanBeCastTo(this Type pluggedType, Type pluginType)
    {
        if (pluggedType == null)
        {
            return false;
        }

        if (pluggedType == pluginType)
        {
            return true;
        }

        return pluginType.IsAssignableFrom(pluggedType);
    }

    private static IEnumerable<Type> FindInterfacesThatClose(this Type pluggedType, Type templateType)
    {
        return FindInterfacesThatClosesCore(pluggedType, templateType).Distinct();
    }

    private static IEnumerable<Type> FindInterfacesThatClosesCore(Type pluggedType, Type templateType)
    {
        if (pluggedType == null) yield break;

        if (!pluggedType.IsConcrete()) yield break;

        if (templateType.IsInterface)
        {
            foreach (
                var interfaceType in
                    pluggedType.GetInterfaces().Where(
                        type => type.IsGenericType && type.GetGenericTypeDefinition() == templateType))
            {
                yield return interfaceType;
            }
        }
        else if (pluggedType.BaseType!.IsGenericType && pluggedType.BaseType!.GetGenericTypeDefinition() == templateType)
        {
            yield return pluggedType.BaseType!;
        }

        if (pluggedType.BaseType == typeof(object)) yield break;

        foreach (var interfaceType in FindInterfacesThatClosesCore(pluggedType.BaseType!, templateType))
        {
            yield return interfaceType;
        }
    }

    private static bool IsConcrete(this Type type)
    {
        return !type.IsAbstract && !type.IsInterface;
    }

    private static void Fill<T>(this List<T> items, T value)
    {
        if (items.Contains(value)) return;
        items.Add(value);
    }

    private static bool IsOpenGeneric(this Type type)
    {
        return type.IsGenericTypeDefinition || type.ContainsGenericParameters;
    }
}
