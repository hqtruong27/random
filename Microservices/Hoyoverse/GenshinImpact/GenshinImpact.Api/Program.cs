using System.Text.Json.Serialization;
using Common.Enum.Hoyoverse;
using GenshinImpact.Api.Mapper;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var environment = builder.Environment;
var configuration = builder.Configuration
   .SetBasePath(builder.Environment.ContentRootPath)
   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
   .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)
   .AddEnvironmentVariables()
   .Build();

//services.Configure<MongoDbSettings>(configuration.GetSection("MongoDb"));
BsonClassMap.RegisterClassMap<GachaHistory>(map =>
{
    map.AutoMap();
    map.MapMember(x => x.RankType).SetSerializer(new EnumSerializer<RankType>(MongoDB.Bson.BsonType.String));
    map.MapMember(x => x.GachaType).SetSerializer(new EnumSerializer<GachaType>(MongoDB.Bson.BsonType.String));
    map.MapMember(x => x.ItemType).SetSerializer(new EnumSerializer<ItemType>(MongoDB.Bson.BsonType.String));
});

services.AddSingleton<IDbContextOptions>(builder.Configuration.GetSection("MongoDb").Get<MongoDbContextOptions>()!);
services.AddHoyoverseDbContext();
services.AddScoped<IGachaHistoryService, GachaHistoryService>();

//var collection = services.BuildServiceProvider().GetRequiredService<MongoDbService>();

//await collection.InsertAsync(new Hoyoverse.Infrastructure.Entities.GachaHistory
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

services.AddAutoMapper(typeof(OrganizationProfile)).AddControllers().AddJsonOptions(o =>
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
