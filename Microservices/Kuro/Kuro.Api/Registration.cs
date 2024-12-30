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
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.CustomSchemaIds(type => type.FullName);
        });

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

        return services;
    }

    public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
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

            await next(context);
        });

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseApiVersioning();
        app.UseRouting();
        app.UseHttpsRedirection();
        app.UseAmbientContext();
        app.UseLoggingChannelEventReader();

        return app;
    }
}