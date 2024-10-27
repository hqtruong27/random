using GenshinImpact.Api;

var builder = WebApplication.CreateBuilder(args);

var (services, configuration) = builder.Configuration();

services
    .AddOptions(configuration)
    .AddAutoMapper()
    .AddSwagger()
    .AddDbContext(configuration)
    .AddDispatcher(typeof(Program).Assembly);

var app = builder.Build();

app.UseMiddleware();

await app.RunAsync();