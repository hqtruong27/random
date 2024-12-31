var builder = WebApplication.CreateBuilder(args);

var (services, configuration, assembly) = await builder.SetupAsync();

services
    .AddOptions(configuration)
    .AddDbContext(configuration, assembly)
    .AddMediatR(assembly)
    .AddValidatorsFromAssembly(assembly, includeInternalTypes: true)
    .AddAutoMapper()
    .AddQuartz()
    .AddExternalService(configuration)
    .AddCustomLogging()
    .AddDomainEventPublisher()
    .AddEventPublisher(builder =>
    {
        builder.UseAwsSnsSqs(configure => configuration.GetSection("Aws").Bind(configure));
    })
    .AddApiVersioning(assembly, version: 1);

var app = builder.Build();

await app
    .UseHttpRequestPipeline()
    .RunAsync();