using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence.Repositories.Abstractions;

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

        //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(QuartzMediatorBehavior<,>));
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

        services.AddRepositories();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(MongoRepository<>));
        services.AddScoped(typeof(IRepository<,>), typeof(MongoRepository<,>));

        return services;
    }
}

public static class WebApplicationExtensions
{
    public static IApplicationBuilder UseAmbientContext(this IApplicationBuilder app)
    {
        return app.UseMiddleware<AmbientContextMiddleware>();
    }
    
    public static IApplicationBuilder UseLoggingChannelEventReader(this IApplicationBuilder app, Action? callback = null)
    {
        app.Use(async (context, next) =>
        {
            try
            {
                await next(context);
            }
            catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
            {
                callback?.Invoke();
            }
        });

        return app;
    }
}