using Hoyolab.Api;

var builder = WebApplication.CreateBuilder(args);

var (services, configuration) = builder.Configuration();

services
    .AddOptions(configuration)
    .AddDbContext(configuration)
    .AddAutoMapper()
    .AddCronQuartz()
    .AddCustomLogging()
    .AddDispatcher()
    .AddSwagger();

var app = builder.Build();

app.UseHttpRequestPipeline();

await app.RunAsync();