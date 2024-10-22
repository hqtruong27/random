using System.Text.Json.Serialization;
using Common.Enum.Hoyoverse;
using GenshinImpact.Api.Mapper;
using Hoyoverse.Infrastructure.Core;
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

BsonClassMap.RegisterClassMap<GachaHistory>(map =>
{
    map.AutoMap();
    map.MapMember(x => x.RankType).SetSerializer(new EnumSerializer<RankType>(MongoDB.Bson.BsonType.String));
    map.MapMember(x => x.GachaType).SetSerializer(new EnumSerializer<GachaType>(MongoDB.Bson.BsonType.String));
    map.MapMember(x => x.ItemType).SetSerializer(new EnumSerializer<ItemType>(MongoDB.Bson.BsonType.String));
});

services.AddSingleton<IDbContextOptions>(configuration.GetSection("MongoDb").Get<MongoDbContextOptions>()!);
services.AddHoyoverseDbContext();
services.AddDispatcher(typeof(Program).Assembly);
services.AddAutoMapper(typeof(OrganizationProfile)).AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

//services.Configure<ForwardedHeadersOptions>(options =>
//{
//    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
//});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerGen();

// comment for reverse proxy
// builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseDeveloperExceptionPage();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

//app.UseForwardedHeaders();
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

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();