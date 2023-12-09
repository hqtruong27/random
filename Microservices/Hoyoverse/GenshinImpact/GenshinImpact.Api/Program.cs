using GenshinImpact.Api.Services;
using GenshinImpact.Api.Settings;
using System.Text.Json.Serialization;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


var services = builder.Services;
var environment = builder.Environment;
var configuration = builder.Configuration
   .SetBasePath(builder.Environment.ContentRootPath)
   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
   .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)
   .AddEnvironmentVariables()
   .Build();

services.Configure<MongoDbSettings>(configuration.GetSection("MongoDb"));
services.AddSingleton<MongoDbService>();

//var collection = services.BuildServiceProvider().GetRequiredService<MongoDbService>();

//await collection.CreateAsync(new Hoyoverse.Infrastructure.Entities.GachaHistory
//{
//    Count = 1,
//    GachaType = Common.Enum.Hoyoverse.GachaType.CharLimited,
//    Id = 2,
//    ItemId = "",
//    ItemType = Common.Enum.Hoyoverse.ItemType.Character,
//    Lang = "vi-VN",
//    Name = "Test",
//    RankType = Common.Enum.Hoyoverse.RankType.Five,
//    Time = DateTime.Now,
//    Uid = ""
//});


services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
