var builder = WebApplication.CreateBuilder(args);

var (services, configuration, assembly) = await builder.SetupAsync();

services
    .AddOptions(configuration)
    .AddDbContext(configuration, assembly)
    .AddMediatR(assembly)
    .AddDomainEventPublisher()
    .AddEventPublisher(builder =>
    {
        builder.UseAwsSnsSqs(configure => configuration.GetSection("Aws").Bind(configure));
    })
    .AddApiVersioning(assembly);

var app = builder.Build();

app.UseHttpRequestPipeline();

await app.RunAsync();
