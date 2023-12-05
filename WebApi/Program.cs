using Microsoft.AspNetCore.HttpOverrides;
using WebApi.Mapper;
using WebApi.Services;
using Microsoft.EntityFrameworkCore.Diagnostics;


var builder = WebApplication.CreateBuilder(args);

var environmentName = builder.Environment.EnvironmentName;
Console.WriteLine(environmentName);

var services = builder.Services;
var configuration = builder.Configuration
   .SetBasePath(builder.Environment.ContentRootPath)
   .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
   .AddEnvironmentVariables()
   .Build();

var connectionString = configuration.GetConnectionString("StatisticsDbContext");
Console.WriteLine(connectionString);

services.AddDbContextPool<StatisticsDbContext>(options =>
{
    options.ConfigureWarnings(builder =>
    {
        builder.Ignore(CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning);
    });

    options.UseNpgsql(connectionString);
});

services.AddReverseProxy().LoadFromConfig(configuration.GetSection("ReverseProxy"));

services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

// Add services to the container.
services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
}).AddJsonTranscoding();
services.AddGrpcReflection();
services.AddAutoMapper(typeof(OrganizationProfile));

var app = builder.Build();

app.MapReverseProxy();
// Configure the HTTP request pipeline.
app.MapGrpcService<UserService>();
app.MapGrpcService<SpendingService>();

if (app.Environment.IsDevelopment())
{
}

app.UseForwardedHeaders();
app.UseHsts();
app.MapGrpcReflectionService();

app.MapGet("/", () => "Bot discord Statistics use gRPC");

await MigrateAsync();

app.Run();

async Task MigrateAsync()
{
    Console.WriteLine("START Migrate");
    using var scope = app.Services.CreateScope();
    var _context = scope.ServiceProvider.GetRequiredService<StatisticsDbContext>();
    await _context.Database.MigrateAsync();
    Console.WriteLine("END:Migrate success");
}