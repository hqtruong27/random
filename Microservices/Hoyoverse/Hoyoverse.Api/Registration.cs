using Hoyoverse.QuartzJobs;
using Infrastructure.Quartz;
using Scalar.AspNetCore;

namespace Hoyoverse;

public static class Registration
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

        services.AddHttpContextAccessor();
        //services.AddOpenApi();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.CustomSchemaIds(type => type.FullName);
        });

        InstallPlaywright();

        return (services, configuration, typeof(Program).Assembly);
    }

    public static void InstallPlaywright()
    {
        Microsoft.Playwright.Program.Main(["install"]);
    }

    public static IServiceCollection AddDbContext(
        this IServiceCollection services,
        IConfiguration configuration,
        Assembly? assembly = null)
    {
        services.AddMongoDbContext<HoyoverseDbContext>(builder =>
        {
            builder.WithOptions(configuration.GetSection("Hoyoverse:MongoDb"));
            if (assembly != null)
            {
                builder.AutoScanMongoConfiguration(assembly);
            }
        });

        return services;
    }

    public static IServiceCollection AddAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(OrganizationProfile));
        services.Configure<JsonOptions>(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        return services;
    }

    public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        services.Configure<QuartzOptions>(options =>
        {
            options.Scheduling.IgnoreDuplicates = true;
        });

        var mongoDbContextOptions = configuration.GetSection("Hoyoverse:MongoDb").Get<MongoDbContextOptions>()!;
        services.AddSingleton<IDbContextOptions>(mongoDbContextOptions);

        return services;
    }

    public static IServiceCollection AddQuartz(this IServiceCollection services)
    {
        services.AddQuartz(q =>
        {
            q.AddJobListener<QuartzJobContextListener>();

            q.SchedulerId = "Scheduler-Core";
            q.UseDefaultThreadPool(tp =>
            {
                tp.MaxConcurrency = 10;
            });

            var checkInJobKey = new JobKey("AutoCheckIn", "Hoyolab");
            q.AddJob<CheckInJob>(checkInJobKey, j => j
                .WithDescription("AutoCheckIn")
                .StoreDurably()
            );

            q.AddTrigger(t => t
                .WithIdentity("AutoCheckIn-Trigger", "Hoyolab")
                .ForJob(checkInJobKey)
                .WithCronSchedule("0 0 1 ? * * *")
                .WithDescription("daily check in job")
            );

            var redeemCodeJobKey = new JobKey("RedeemCodeGenshinImpact", "Hoyolab");
            q.AddJob<RedeemCodeCommandJob>(redeemCodeJobKey, j => j
                .WithDescription("RedeemCodeGenshinImpact")
                .StoreDurably()
            );

            q.AddTrigger(t => t
                .WithIdentity("RedeemCodeGenshinImpact-Trigger", "Hoyolab")
                .ForJob(redeemCodeJobKey)
                .StartNow()
                .WithDescription("redeem code job")
            );
        });

        services.AddQuartzHostedService(opt =>
        {
            opt.WaitForJobsToComplete = true;
        });

        return services;
    }

    public static IServiceCollection AddCustomLogging(this IServiceCollection services)
    {
        LogProvider.SetCurrentLogProvider(new ConsoleLogProvider());

        return services;
    }

    public static WebApplication UseHttpRequestPipeline(this WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            if (context.Request.Path.Value == "/favicon.ico")
            {
                context.Response.StatusCode = StatusCodes.Status200OK;
                return;
            }

            await next.Invoke(context);
        });

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseApiVersioning();
        app.UseRouting();
        app.MapEndpoints();
        app.UseAmbientContext();
        app.UseLoggingChannelEventReader();

        return app;
    }
}

internal class ConsoleLogProvider : ILogProvider
{
    public Logger GetLogger(string name)
    {
        return (level, func, _, parameters) =>
        {
            if (level >= Quartz.Logging.LogLevel.Info && func != null)
            {
                Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] [{level}] {func()}", parameters);
            }
            return true;
        };
    }

    public IDisposable OpenNestedContext(string message)
    {
        throw new NotImplementedException();
    }

    public IDisposable OpenMappedContext(string key, object value, bool destructure = false)
    {
        throw new NotImplementedException();
    }
}
