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
    .AddEndpoints();

await builder
    .Build()
    .UseHttpRequestPipeline()
    .RunAsync();