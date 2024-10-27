using GenshinImpact.Api.Mapper;
using GenshinImpact.Infrastructure.Common.Settings;
using System.Text.Json.Serialization;
using GenshinImpact.Infrastructure;

namespace GenshinImpact.Api;

public static class Registration
{
    public static (IServiceCollection, IConfiguration) Configuration(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;
        var environment = builder.Environment;
        var configuration = builder.Configuration
            .SetBasePath(builder.Environment.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        // comment for reverse proxy
        // builder.Services.AddEndpointsApiExplorer();

        return (services, configuration);
    }

    public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var mongoDbContextOptions = configuration.GetSection("MongoDb").Get<MongoDbContextOptions>()!;
        return services.AddGenshinImpactDbContext(mongoDbContextOptions);
    }

    public static IServiceCollection AddAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(OrganizationProfile)).AddControllers().AddJsonOptions(o =>
        {
            o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        return services;
    }

    public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        var mongoDbContextOptions = configuration.GetSection("MongoDb").Get<MongoDbContextOptions>()!;
        services.AddSingleton<IDbContextOptions>(mongoDbContextOptions);

        return services;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        return services.AddSwaggerGen();
    }

    public static IApplicationBuilder UseMiddleware(this WebApplication app)
    {
        app.UseHttpRequestPipeline();

        app.UseSwaggerGen();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        return app;
    }

    public static IApplicationBuilder UseHttpRequestPipeline(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseDeveloperExceptionPage();
        }

        app.UseForwardedHeaders();
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

        app.MapGet("/", () => "Genshin Impact Api");

        return app;
    }

    public static IApplicationBuilder UseSwaggerGen(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        return app;
    }
}
