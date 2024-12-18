using System.Threading.Channels;

namespace Microsoft.Extensions.DependencyInjection;

public static class EventBusServiceCollectionExtensions
{
    public static IServiceCollection AddEventPublisher(
        this IServiceCollection services,
        Action<EventBusOptionsBuilder> builder,
        ServiceLifetime lifetime = ServiceLifetime.Singleton
        )
    {
        var optionsBuilder = new EventBusOptionsBuilder(services);
        builder.Invoke(optionsBuilder);

        var factory = optionsBuilder.Build();

        services.Add(new(
            typeof(IEventPublisher),
            factory,
            lifetime
            ));

        services.AddHostedService<EventSubscriberBackgroundService>();

        return services;
    }

    public static IServiceCollection AddDomainEventPublisher(this IServiceCollection services)
    {
        services.AddSingleton(Channel.CreateUnbounded<Func<IServiceProvider, CancellationToken, Task>>(new()
        {
            SingleWriter = false,
            SingleReader = true
        }));

        services.AddHostedService<ChannelEventReader>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        return services;
    }
}