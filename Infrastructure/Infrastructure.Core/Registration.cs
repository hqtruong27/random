using Microsoft.Playwright;

namespace Microsoft.Extensions.DependencyInjection;

public static class Registration
{
    public static IServiceCollection AddMediatR(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(assemblies);
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        //services.AddScoped<IPublisher, AsyncPublisherDecorator>();
        //services.AddScoped(typeof(IAsyncPublishQueue<>), typeof(AsyncPublishQueue<>));

        return services;
    }

    public static IServiceCollection AddMongoDbContext<TContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> builder)
        where TContext : IDbContext
    {
        builder.Invoke(new DbContextOptionsBuilder(services));
        services.AddSingleton(typeof(TContext));
        services.AddSingleton(typeof(IDbContext), typeof(TContext));

        return services;
    }

    public static IServiceCollection AddEventHandlers(this IServiceCollection services, Assembly assembly)
    {
        // 1. Register the EventDispatcher itself
        //services.AddSingleton<IEventDispatcher>(new EventDispatcher(services.BuildServiceProvider(), assembly));

        // 2. Find all IEventHandler<> implementations in the assembly
        var handlerTypes = assembly.GetTypes()
            .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>)))
            .ToList(); // Materialize the list to avoid multiple enumeration

        // 3. Register each handler with the DI container
        foreach (var handlerType in handlerTypes)
        {
            var eventType = handlerType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                .GetGenericArguments()[0];

            // Register the handler with its corresponding interface
            services.AddScoped(typeof(IEventHandler<>).MakeGenericType(eventType), handlerType);
        }

        return services;
    }
}

public static class WebApplicationExtensions
{
    public static IApplicationBuilder UseAmbientContext(this IApplicationBuilder app)
    {
        return app.UseMiddleware<AmbientContextMiddleware>();
    }

    public static IApplicationBuilder UseLoggingChannelEventReader(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            try
            {
                await next(context);
            }
            catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
            {
                var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("Request was canceled.");
            }
        });

        return app;
    }
}