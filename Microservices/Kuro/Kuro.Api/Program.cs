var builder = WebApplication.CreateBuilder(args);

var (services, configuration, assembly) = await builder.SetupAsync();

services
    .AddOptions(configuration)
    .AddDbContext(configuration, assembly)
    .AddMediatR(assembly)
    .AddDomainEventPublisher()
    .AddEventPublisher(builder =>
    {
        builder.RegisterEventHandlerFromAssembly(assembly);
        builder.UseAwsSnsSqs(configure => configuration.GetSection("Aws").Bind(configure));
    })
    .AddEndpoints();

var app = builder.Build();

app.UseHttpRequestPipeline().MapEndpoints();

await app.RunAsync();
