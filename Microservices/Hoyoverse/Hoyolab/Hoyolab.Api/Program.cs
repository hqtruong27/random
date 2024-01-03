using Hoyolab.Api.Job;
using Quartz;
using Quartz.Logging;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var services = builder.Services;
var environment = builder.Environment;
var configuration = builder.Configuration
   .SetBasePath(builder.Environment.ContentRootPath)
   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
   .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)
   .AddEnvironmentVariables()
   .Build();

Console.WriteLine($"Current: {DateTimeOffset.Now}");

services.Configure<QuartzOptions>(options =>
{
    options.Scheduling.IgnoreDuplicates = true;
});

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
        //.WithCronSchedule("0 0 1 ? * * * ")
        .StartNow()
        .WithDescription("daily check in job")
    );
});

services.AddQuartzHostedService(opt =>
{
    opt.WaitForJobsToComplete = true;
});

LogProvider.SetCurrentLogProvider(new ConsoleLogProvider());

services.AddSingleton<IDbContextOptions>(builder.Configuration.GetSection("MongoDb").Get<MongoDbContextOptions>()!);
services.AddHoyoverseDbContext();
services.AddDispatcher(Assembly.GetExecutingAssembly());

services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

app.MapDefaultEndpoints();

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

class ConsoleLogProvider : ILogProvider
{
    public Logger GetLogger(string name)
    {
        return (level, func, exception, parameters) =>
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

