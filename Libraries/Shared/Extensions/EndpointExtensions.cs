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

        var apiRouteTypes = AppDomain.CurrentDomain
           .GetAssemblies()
           .SelectMany(assembly => assembly.GetTypes())
           .Where(type => type.IsClass && type.HasApiRoute())
           .ToList();

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

        builder.MapRouteAttributes();

        return app;
    }

    public static IEndpointRouteBuilder MapRouteAttributes(this IEndpointRouteBuilder app)
    {
        var apiRouteTypes = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && type.HasApiRoute())
            .ToList();

        foreach (var routeType in apiRouteTypes)
        {
            app.MapRouteForType(routeType);
        }

        return app;
    }

    private static void MapRouteForType(this IEndpointRouteBuilder app, Type routeType)
    {
        var routeAttr = routeType.GetRouteAttribute();
        var tagsAttr = routeType.GetTagsAttribute();
        var httpMethodAttr = routeType.GetHttpMethodAttributeOfDefault();
        var httpMethod = httpMethodAttr.HttpMethods.First();
        var route = routeAttr?.Template ?? httpMethodAttr.Template
            ?? throw new InvalidOperationException(
                $"HTTP method route not found for {routeType.Name}"
                );

        var dynamicDelegate = DynamicDelegateFactory.Create(httpMethod, routeType, app.ServiceProvider);
        app.MapMethods(route, [httpMethod], dynamicDelegate)
           .WithOpenApi(operation =>
           {
               if (HttpMethodHelper.IsHttpMethodSupported(httpMethod))
               {
                   operation.Parameters = OpenApiExtensions.GenerateParameters(routeType);
               }

               return operation;
           })
           .WithTags(
                tagsAttr?.Tags
                .Select(x => x.Pascalize())
                .ToArray() ?? route.DefaultTags(routeType.Assembly)
                )
           .Produces(
            StatusCodes.Status200OK,
            routeType.GetIRequestInterface()?.GetGenericArguments()[0]
            );
    }
}