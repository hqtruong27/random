namespace Shared.Extensions;

public static class EndpointExtensions
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly)
    {
        var serviceDescriptors = assembly.DefinedTypes
             .Where(type => type.IsConcreteImplementationOf<IEndpoint>())
             .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
             .ToArray();

        services.TryAddEnumerable(serviceDescriptors);

        return services;
    }

    public static IApplicationBuilder MapEndpoints(this WebApplication app, RouteGroupBuilder? routeGroupBuilder = default)
    {
        var endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();

        IEndpointRouteBuilder builder = routeGroupBuilder is null ? app : routeGroupBuilder;

        foreach (var endpoint in endpoints)
        {
            endpoint.MapEndpoint(builder);
        }

        builder.MapRouteAttributes(app.Services);

        return app;
    }

    public static IEndpointRouteBuilder MapRouteAttributes(this IEndpointRouteBuilder app, IServiceProvider services)
    {
        var apiRouteTypes = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && type.HasApiRoute())
            .ToList();

        var sender = services.GetRequiredService<ISender>();
        foreach (var routeType in apiRouteTypes)
        {
            app.MapRouteForType(routeType, sender);
        }

        return app;
    }

    private static void MapRouteForType(this IEndpointRouteBuilder app, Type routeType, ISender sender)
    {
        var routeAttr = routeType.GetRouteAttribute();
        var tagsAttr = routeType.GetTagsAttribute();
        var httpMethodAttr = routeType.GetHttpMethodAttributeOfDefault();
        var httpMethod = httpMethodAttr.HttpMethods.First();
        var route = routeAttr?.Template ?? httpMethodAttr.Template
            ?? throw new InvalidOperationException(
                $"HTTP method route not found for {routeType.Name}"
                );

        var dynamicDelegate = DynamicDelegateFactory.Create(httpMethod, routeType, sender);
        app.MapMethods(route, [httpMethod], dynamicDelegate)
           .WithOpenApi()
           .WithTags(
                tagsAttr?.Tags
                .Select(x => x.Pascalize())
                .ToArray() ?? route.DefaultTags(routeType.Assembly)
                );
    }
}
