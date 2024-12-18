using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using Kuro.Features.WutheringWaves.Events;

namespace Kuro;

public static class ServiceCollectionExtensions
{
    public static async Task<(IServiceCollection, IConfiguration, Assembly)> SetupAsync(this IHostApplicationBuilder builder)
    {
        var doppler = new DopplerClient();
        var jsonStream = await doppler.FetchSecretsAsStreamAsync();

        var services = builder.Services;
        var environment = builder.Environment;
        var configuration = builder.Configuration
            .SetBasePath(builder.Environment.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)
            .AddJsonStream(jsonStream)
            .AddEnvironmentVariables()
            .Build();

        services.AddOpenApi();

        return (services, configuration, typeof(Program).Assembly);
    }

    public static IServiceCollection AddDbContext(
        this IServiceCollection services,
        IConfiguration configuration,
        Assembly? assembly = null)
    {
        services.AddMongoDbContext<KuroDbContext>(builder =>
        {
            builder.WithOptions(configuration.GetSection("Kuro:MongoDb"));
            if (assembly != null)
            {
                builder.AutoScanMongoConfiguration(assembly);
            }
        });

        services.AddRepositories();

        return services;
    }

    public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton(typeof(IRepository<>), typeof(MongoRepository<>));
        services.AddSingleton(typeof(IRepository<,>), typeof(MongoRepository<,>));

        return services;
    }

    public static IServiceCollection AddEndpoints(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new(1);
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'V";
            options.SubstituteApiVersionInUrl = true;
        });

        services.AddEndpoints(typeof(Program).Assembly);

        return services;
    }
}

public static class WebApplicationExtensions
{
    public static WebApplication UseHttpRequestPipeline(this WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            if (context.Request.Path.Value == "/favicon.ico")
            {
                context.Response.StatusCode = StatusCodes.Status200OK;
                return;
            }

            await next.Invoke();
        });

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseHttpsRedirection();
        app.UseAmbientContext();
        app.UseLoggingChannelEventReader();

        return app;
    }

    public static void MapEndpoints(this WebApplication app)
    {
        var apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();

        var versionedGroup = app
            .MapGroup("api/v{version:apiVersion}")
            .WithApiVersionSet(apiVersionSet);

        app.MapEndpoints(versionedGroup);
    }
}