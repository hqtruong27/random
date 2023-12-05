using WebApi.Mapper;
using WebApi.Services;

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
        builder.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning);
    });

    options.UseNpgsql(connectionString);
});

// Add services to the container.
services.AddGrpc().AddJsonTranscoding();
services.AddGrpcReflection();
services.AddAutoMapper(typeof(OrganizationProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<UserService>();
app.MapGrpcService<SpendingService>();

if (app.Environment.IsDevelopment())
{
}

app.MapGrpcReflectionService();

app.MapGet("/", () => "Bot discord Statistics use gRPC");
app.MapGet("/grpc", () => "Bot discord Statistics use gRPC");

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