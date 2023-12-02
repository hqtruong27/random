using Grpc.Net.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Models.Settings;
using Spending.Services;

Console.WriteLine("Hello, World!");

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("StatisticsDbContext");

builder.Services.AddDbContextPool<StatisticsDbContext>(options =>
{
    options.ConfigureWarnings(builder =>
    {
        builder.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning);
    });

    options.UseNpgsql(connectionString);
});

builder.Services.AddScoped<ISpendingService, SpendingService>();
var app = builder.Build();
var e = builder.Configuration.GetSection("Statistics").Get<StatisticSettings>();
using var channel = GrpcChannel.ForAddress("https://localhost:7072");
var spending = new WebApi.Protos.Spending.SpendingClient(channel);
var response = await spending.GetAsync(new GetRequest
{
    Id = "A291FAB5-2747-4D8C-9A10-69406F2AA490",
});
var a = builder.Services.BuildServiceProvider().GetRequiredService<ISpendingService>();
Console.WriteLine(response.Id);

app.Run();