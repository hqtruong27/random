using FluentValidation;
using Infrastructure.Core.Behaviors;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Random.ExternalService;
using StarRail.Core.Entities;
using StarRail.Core.Enums;
using StarRail.Core.Interfaces.Repositories;
using StarRail.Infrastructure.Common.Settings;
using StarRail.Infrastructure.Persistence;
using StarRail.Infrastructure.Persistence.Repositories;

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

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

BsonClassMap.RegisterClassMap<GachaHistory>(map =>
{
    map.AutoMap();
    map.MapMember(x => x.RankType).SetSerializer(new EnumSerializer<RankType>(MongoDB.Bson.BsonType.String));
    map.MapMember(x => x.GachaType).SetSerializer(new EnumSerializer<GachaType>(MongoDB.Bson.BsonType.String));
    map.MapMember(x => x.ItemType).SetSerializer(new EnumSerializer<ItemType>(MongoDB.Bson.BsonType.String));
});

services.AddExternalService(configuration);
services.AddSingleton<IDbContextOptions>(builder.Configuration.GetSection("MongoDb").Get<MongoDbContextOptions>()!);
services.AddSingleton<IStarRailDbContext, StarRailDbContext>();
services.AddScoped(typeof(IRepository<,>), typeof(MongoRepository<,>));

var assembly = typeof(Program).Assembly;
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});
builder.Services.AddValidatorsFromAssembly(assembly);

var app = builder.Build();

app.MapDefaultEndpoints();

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
