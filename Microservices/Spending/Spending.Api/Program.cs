using Microsoft.AspNetCore.HttpOverrides;
using Spending.Api.Services;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Spending.Api.Mapper;

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

services.AddDbContextPool<StatisticsDbContext>(options =>
{
    options.ConfigureWarnings(builder =>
    {
        builder.Ignore(CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning);
    });

    options.UseNpgsql(connectionString);
});

//services.AddReverseProxy().LoadFromConfig(configuration.GetSection("ReverseProxy"));

//services.Configure<ForwardedHeadersOptions>(options =>
//{
//    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
//});

// Add services to the container.
services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
}).AddJsonTranscoding();
services.AddGrpcReflection();
services.AddAutoMapper(typeof(OrganizationProfile));

var app = builder.Build();

//app.MapReverseProxy();
// Configure the HTTP request pipeline.
app.MapGrpcService<UserService>();
app.MapGrpcService<SpendingService>();

app.UseDeveloperExceptionPage();

if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

//app.UseForwardedHeaders();
app.UseHsts();

app.Use(async (context, next) =>
{
    if (context.Request.Path.Value == "/favicon.ico")
    {
        context.Response.StatusCode = StatusCodes.Status200OK;
        return;
    }

    // No favicon, call next middleware
    await next.Invoke();
});

app.MapGet("/", () => "Bot discord Statistics use gRPC");

await MigrateAsync();

await app.RunAsync();

async Task MigrateAsync()
{
    Console.WriteLine("START Migrate");
    using var scope = app.Services.CreateScope();
    var _context = scope.ServiceProvider.GetRequiredService<StatisticsDbContext>();
    await _context.Database.MigrateAsync();
    Console.WriteLine("END:Migrate success");
}
