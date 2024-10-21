using System.Text.Json;
using DisCatSharp;
using DisCatSharp.ApplicationCommands;
using DisCatSharp.CommandsNext;
using DisCatSharp.Entities;
using DisCatSharp.Enums;
using DisCatSharp.Lavalink;
using DisCatSharp.Net;
using Discord.Bot.Lavalink;
using Discord.Bot.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

var services = builder.Services;
var environment = builder.Environment;
var configuration = builder.Configuration
   .SetBasePath(builder.Environment.ContentRootPath)
   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
   .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)
   .AddEnvironmentVariables()
   .Build();

var discordSettings = configuration.GetSection(nameof(DiscordSettings)).Get<DiscordSettings>()!;
var statisticSettings = configuration.GetSection(nameof(StatisticSettings)).Get<StatisticSettings>()!;
var hoyolabSettings = configuration.GetSection(nameof(HoyolabSettings)).Get<HoyolabSettings>()!;
var lavalinkSettings = configuration.GetSection(nameof(LavalinkSettings)).Get<LavalinkSettings>()!;

services.AddSingleton(discordSettings);
services.AddSingleton(statisticSettings);
services.AddSingleton(hoyolabSettings);
services.AddSingleton(lavalinkSettings);
var discord = new DiscordClient(new DiscordConfiguration
{
    Token = discordSettings.Token,
    TokenType = TokenType.Bot,
    Intents = DiscordIntents.AllUnprivileged
            | DiscordIntents.MessageContent
            | DiscordIntents.Guilds
            | DiscordIntents.GuildIntegrations
            | DiscordIntents.GuildVoiceStates
            | DiscordIntents.GuildMessages
            | DiscordIntents.GuildMembers
            | DiscordIntents.DirectMessages
            | DiscordIntents.DirectMessageReactions,
    MinimumLogLevel = LogLevel.Trace,
    ShowReleaseNotesInUpdateCheck = false
});

var assembly = typeof(Program).Assembly;
var commands = discord.UseApplicationCommands();
if (environment.IsProduction())
{
    commands.RegisterGlobalCommands(assembly);
    commands.RegisterGuildCommands(assembly, discordSettings.GuildId);
}
else
{
    commands.RegisterGuildCommands(assembly, discordSettings.GuildId);
}

discord.UseCommandsNext(new CommandsNextConfiguration
{
    StringPrefixes = ["~"],
    ServiceProvider = services.BuildServiceProvider(),
}).RegisterCommands(assembly);

var endpoint = new ConnectionEndpoint
{
    Hostname = lavalinkSettings.Host,
    Port = lavalinkSettings.Port
};

var lavaLinkConfig = new LavalinkConfiguration
{
    Password = lavalinkSettings.Password,
    RestEndpoint = endpoint,
    SocketEndpoint = endpoint,
    EnableBuiltInQueueSystem = true
};

Console.WriteLine(lavalinkSettings);
Console.WriteLine("lavalinkSettings {0}", JsonSerializer.Serialize(lavalinkSettings));

await discord.ConnectAsync();

try
{
    await discord.UseLavalink().ConnectAsync(lavaLinkConfig);
}
catch
{
    // ignored
}

await discord.UpdateStatusAsync(new DiscordActivity
{
    Id = "1",
    ActivityType = ActivityType.Playing,
    Name = $"Node {Random.Shared.Next(1, 6)}",
});

//async Task StartAsync()
//{
//    DiscordSocketConfig discordSocketConfig = new()
//    {
//        GatewayIntents = GatewayIntents.MessageContent
//                       | GatewayIntents.GuildMessages
//                       | GatewayIntents.Guilds
//                       | GatewayIntents.GuildMembers
//                       | GatewayIntents.DirectMessages
//                       | GatewayIntents.GuildVoiceStates
//                       | GatewayIntents.GuildIntegrations
//                       | GatewayIntents.DirectMessageReactions,
//        AlwaysDownloadUsers = true,
//        ConnectionTimeout = 60 * 1000,
//        UseInteractionSnowflakeDate = false,
//    };

//    CommandServiceConfig commandServiceConfig = new()
//    {
//        DefaultRunMode = Discord.Commands.RunMode.Async
//    };

//    InteractionServiceConfig interactionConfig = new()
//    {
//        DefaultRunMode = Discord.Interactions.RunMode.Async
//    };

//    DiscordSocketClient client = new(discordSocketConfig);

//    CommandService commands = new(commandServiceConfig);

//    InteractionService interactions = new(client, config: interactionConfig);

//    var discordSettings = configuration.GetSection(nameof(DiscordSettings)).Get<DiscordSettings>()!;
//    var statisticSettings = configuration.GetSection(nameof(StatisticSettings)).Get<StatisticSettings>()!;
//    var hoyolabSettings = configuration.GetSection(nameof(HoyolabSettings)).Get<HoyolabSettings>()!;
//    var services = new ServiceCollection()
//           .AddLogging(builder =>
//           {
//               builder.AddConsole();
//           })
//           .AddSingleton(client)
//           .AddSingleton(commands)
//           .AddSingleton(interactions)
//           .AddSingleton(statisticSettings)
//           .AddSingleton(hoyolabSettings)
//           .AddSingleton<CommandHandler>()
//           .AddSingleton(new DiscordSettings()
//           {
//               ClientId = discordSettings.ClientId,
//               GuildId = discordSettings.GuildId,
//               Token = discordSettings.Token,
//               Channel = GrpcChannel.ForAddress(statisticSettings.Gateway),
//           })
//           .BuildServiceProvider();

//    client.Log += Logging.LogAsync;
//    commands.Log += Logging.LogAsync;
//    interactions.Log += Logging.LogAsync;
//    client.Ready += ReadyAsync;

//    await client.LoginAsync(TokenType.Bot, discordSettings.Token);

//    await client.StartAsync();

//    await services.GetRequiredService<CommandHandler>().InitializeAsync();

//    while (client.ConnectionState != Discord.ConnectionState.Connected) { }

//    await client.SetActivityAsync(new Game($"{Playing()}..."
//        , ActivityType.Playing
//        , ActivityProperties.Play
//        , "no comments"));

//    string Playing()
//    {
//        var playing = environment.EnvironmentName;
//        if (environment.IsProduction())
//        {
//            playing = $"on node {Random.Shared.Next(1, 20)}...";
//        }

//        return playing;
//    }

//    await ReadyAsync();
//    async Task ReadyAsync()
//    {
//        try
//        {
//            //// Now that we have our builder, we can call the CreateApplicationCommandAsync method to make our slash command.
//            //await guild.CreateApplicationCommandAsync(guildCommand.Build());

//            //// With global commands we don't need the guild.
//            //// await client.CreateGlobalApplicationCommandAsync(globalCommand.Build());
//            //// Using the ready event is a simple implementation for the sake of the example. Suitable for testing and development.
//            //// For a production bot, it is recommended to only run the CreateGlobalApplicationCommandAsync() once for each command.
//            ///

//            if (!environment.IsProduction())
//            {
//                await client.SetStatusAsync(UserStatus.Idle);
//                await interactions.RegisterCommandsToGuildAsync(discordSettings.GuildId);
//            }
//            else
//            {
//                // this method will add commands globally, but can take around an hour
//                await interactions.RegisterCommandsGloballyAsync();
//            }
//        }
//        catch (HttpException exception)
//        {
//            // If our command was invalid, we should catch an ApplicationCommandException.
//            // This exception contains the path of the error as well as the error message.
//            // You can serialize the Error field in the exception to get a visual of where your error is.
//            var json = JsonSerializer.Serialize(exception.Errors);

//            // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
//            Console.WriteLine(json);
//        }

//        Console.WriteLine($"Connected as -> [{client.CurrentUser}] :)");
//    }
//}

var app = builder.Build();

//await StartAsync();

await app.RunAsync();

