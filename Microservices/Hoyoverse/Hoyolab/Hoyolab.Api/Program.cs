using Hoyolab.Services.Interfaces;
using Hoyolab.Services.Services;
using Hoyoverse.Infrastructure.Common.Settings;
using Hoyoverse.Infrastructure.Core;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var environment = builder.Environment;
var configuration = builder.Configuration
   .SetBasePath(builder.Environment.ContentRootPath)
   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
   .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)
   .AddEnvironmentVariables()
   .Build();


services.AddSingleton<IDbContextOptions>(builder.Configuration.GetSection("MongoDb").Get<MongoDbContextOptions>()!);
services.AddHoyoverseDbContext();
services.AddScoped<ICheckInService, CheckInService>();

services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();


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

app.MapGet("/", () => "Hoyolab Api");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
