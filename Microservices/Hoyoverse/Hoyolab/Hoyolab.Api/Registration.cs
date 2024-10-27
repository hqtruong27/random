using Quartz;
using Quartz.Logging;
using System.Reflection;

namespace Hoyolab.Api;

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
        return (services, configuration);
    }

    public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        // BsonSerializer.RegisterSerializer(new StringSerializer(BsonType.ObjectId));

        var options = configuration.GetSection("MongoDb").Get<MongoDbContextOptions>()!;
        services.AddSingleton<IMongoClient>(new MongoClient(options.ConnectionString));
        services.AddSingleton(new MongoClient(options.ConnectionString).GetDatabase(options.Database));
        services.AddSingleton<MongoDbContext>();
        services.AddSingleton<IDatabaseContext, MongoDbContext>();

        return services;
    }

    public static IServiceCollection AddAutoMapper(this IServiceCollection services)
    {
        //services.AddAutoMapper(typeof(OrganizationProfile)).AddControllers().AddJsonOptions(o =>
        //{
        //    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        //});

        return services;
    }

    public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        services.Configure<QuartzOptions>(options =>
        {
            options.Scheduling.IgnoreDuplicates = true;
        });

        var mongoDbContextOptions = configuration.GetSection("MongoDb").Get<MongoDbContextOptions>()!;
        services.AddSingleton<IDbContextOptions>(mongoDbContextOptions);

        return services;
    }

    public static IServiceCollection AddCronQuartz(this IServiceCollection services)
    {
        services.AddQuartz(q =>
        {
            q.SchedulerId = "Scheduler-Core";
            q.UseDefaultThreadPool(tp =>
            {
                tp.MaxConcurrency = 10;
            });

            var jobKey = new JobKey("job", "hoyolab");
            q.AddJob<CheckInJob>(jobKey, j => j
                .WithDescription("job")
            );

            q.AddTrigger(t => t
                .WithIdentity("trigger-job", "hoyolab")
                .ForJob(jobKey)
                .WithCronSchedule("0 0 1 ? * * *")
                .WithDescription("daily check in job")
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

    public static IServiceCollection AddDispatcher(this IServiceCollection services)
    {
        return services.AddDispatcher(Assembly.GetExecutingAssembly());
    }

    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddControllers();
        return services.AddSwaggerGen();
    }

    public static IApplicationBuilder UseHttpRequestPipeline(this WebApplication app)
    {
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
                Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] [" + level + "] " + func(), parameters);
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