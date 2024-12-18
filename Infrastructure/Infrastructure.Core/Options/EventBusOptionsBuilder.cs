using NATS.Client;
using Shared.Extensions;

namespace Infrastructure.Options;

public class EventBusOptionsBuilder(IServiceCollection services)
{
    private Func<IServiceProvider, Type>? _eventPublisherFactory;
    private string? _lastProvider;

    internal Type Build()
    {
        if (_eventPublisherFactory == null)
        {
            throw new InvalidOperationException(
                "No event bus provider has been configured. Call one of the Use[ProviderName] methods to configure a provider.");
        }

        return _eventPublisherFactory.Invoke(services.BuildServiceProvider());
    }

    private void SetProvider(string provider, Func<IServiceProvider, Type>? factory)
    {
        if (_eventPublisherFactory != null)
        {
            Console.WriteLine($"Warning: Overriding previously configured event bus provider '{_lastProvider}' with '{provider}'.");
        }

        _lastProvider = provider;
        _eventPublisherFactory = factory;
    }

    public EventBusOptionsBuilder UseAwsSnsSqs(Action<AwsOptions> configure)
    {
        SetProvider("AwsSnsSqs", (serviceProvider) =>
        {
            var options = new AwsOptions();
            configure.Invoke(options);

            var region = RegionEndpoint.GetBySystemName(options.Region);
            services
                .Configure<AwsOptions>(o => o = options)
                .AddSingleton(options);

            services.AddSingleton<IAmazonSQS>(new AmazonSQSClient(
                options.AccessKey,
                options.SecretKey,
                region
                ));

            services.AddSingleton<IAmazonSimpleNotificationService>(
                new AmazonSimpleNotificationServiceClient(
                    options.AccessKey,
                    options.SecretKey,
                    region
                    ));

            services.AddSingleton<IEventSubscriber, SqsEventSubscriber>();

            return typeof(AmazonEventPublisher);
        });

        return this;
    }

    public EventBusOptionsBuilder UseNats(Action<NatsOptions> configure)
    {
        SetProvider("Nats", (_) =>
        {
            var options = new NatsOptions();
            configure.Invoke(options);

            var cf = new ConnectionFactory();
            var connection = cf.CreateConnection(options.Url);
            services.AddSingleton(connection);
            services.AddSingleton<IEventSubscriber, NatsEventSubscriber>();

            return typeof(NatsEventPublisher);
        });

        return this;
    }

    public EventBusOptionsBuilder RegisterEventHandlerFromAssembly(Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        // 1. Find all IEventHandler<> implementations in the assembly
        var handlerTypes = assembly.GetTypes()
            .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>)))
            .ToList(); // Materialize the list to avoid multiple enumeration

        // 2. Register each handler with the DI container
        //should be at the top because the Event Type Resolver will scan for registered service handlers.
        foreach (var factory in handlerTypes)
        {
            var eventType = factory.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                .GetGenericArguments()[0];

            // Register the handler with its corresponding interface
            var descriptor = new ServiceDescriptor(
                typeof(IEventHandler<>).MakeGenericType(eventType),
                factory,
                lifetime
                );

            services.Add(descriptor);
        }

        var serviceProvider = services.BuildServiceProvider();

        Dictionary<string, Type> eventTypes = [];
        assembly
            .GetTypes()
            .Where(t => typeof(IEvent).IsAssignableFrom(t))
            .Select((type, index) =>
            {
                var attribute = type.GetCustomAttribute<EventAttribute>();
                var order = attribute?.Order ?? index;
                var name = attribute?.Name ?? type.Name;
                return (Type: type, Order: order, Name: name);
            })
            .OrderBy(x => x.Order)
            .ForEach(x => eventTypes[x.Name] = x.Type);

        var eventTypeResolver = new EventTypeResolver(eventTypes);
        foreach (var eventType in eventTypes.Values.Distinct())
        {
            var handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
            var handlers = serviceProvider.GetServices(handlerType);

            foreach (var handler in handlers)
            {
                var delegateType = typeof(Func<,,>).MakeGenericType(eventType, typeof(CancellationToken), typeof(Task));
                var handleMethod = handler!.GetType().GetMethod("Handle")!;
                var handlerDelegate = Delegate.CreateDelegate(delegateType, handler, handleMethod);

                eventTypeResolver.RegisterHandler(
                    eventType,
                    (@event, cancellationToken) 
                        => (Task)handlerDelegate.DynamicInvoke(@event, cancellationToken)!
                        );
            }
        }

        services.AddSingleton(eventTypeResolver);
        services.AddSingleton<IEventDispatcher, DelegateEventDispatcher>();

        return this;
    }

    public EventBusOptionsBuilder RegisterEventHandlerFromAssemblyContaining<T>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        return RegisterEventHandlerFromAssembly(typeof(T).Assembly, lifetime);
    }

    //public void UseRabbitMQ(Action<RabbitMqOptions> configure)
    //{
    //    var options = new RabbitMqOptions();
    //    configure(options);
    //    Services.AddSingleton(options);
    //    Services.AddSingleton<IEventPublisher, RabbitMQEventBus>();
    //}

    //public void UseAzureServiceBus(Action<AzureServiceBusOptions> configure)
    //{
    //    var options = new AzureServiceBusOptions();
    //    configure(options);
    //    Services.AddSingleton(options);
    //    Services.AddSingleton<IEventPublisher, AzureServiceBusEventBus>();
    //}

    //public void UseGooglePubSub(Action<GooglePubSubOptions> configure)
    //{
    //    var options = new GooglePubSubOptions();
    //    configure(options);
    //    Services.AddSingleton(options);
    //    Services.AddSingleton<IEventPublisher, GooglePubSubEventBus>();
    //}

    //public void UseNats(Action<NatsOptions> configure)
    //{
    //    var options = new NatsOptions();
    //    configure(options);
    //    Services.AddSingleton(options);
    //    Services.AddSingleton<IEventPublisher, NatsEventBus>();
    //}
}
