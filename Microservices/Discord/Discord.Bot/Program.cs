using Discord.Net;
using Grpc.Net.Client;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Discord.Commands;
using Discord.Interactions;
using Microsoft.Extensions.Logging;
using Discord.Rest;

var builder = Host.CreateApplicationBuilder(args);

var services = builder.Services;
var environment = builder.Environment;
var configuration = builder.Configuration
   .SetBasePath(builder.Environment.ContentRootPath)
   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
   .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)
   .AddEnvironmentVariables()
   .Build();

async Task StartAsync()
{
    DiscordSocketConfig discordSocketConfig = new()
    {
        GatewayIntents = GatewayIntents.MessageContent
                       | GatewayIntents.GuildMessages
                       | GatewayIntents.Guilds
                       | GatewayIntents.DirectMessages
                       | GatewayIntents.GuildVoiceStates
                       | GatewayIntents.GuildIntegrations
                       | GatewayIntents.DirectMessageReactions,
        AlwaysDownloadUsers = true,
        ConnectionTimeout = 60 * 1000,
        UseInteractionSnowflakeDate = false,
    };

    CommandServiceConfig commandServiceConfig = new()
    {
        DefaultRunMode = Discord.Commands.RunMode.Async
    };

    InteractionServiceConfig interactionConfig = new()
    {
        DefaultRunMode = Discord.Interactions.RunMode.Async
    };

    DiscordSocketClient client = new(discordSocketConfig);

    CommandService commands = new(commandServiceConfig);

    InteractionService interactions = new(client);

    var discordSettings = configuration.GetSection(nameof(DiscordSettings)).Get<DiscordSettings>()!;
    var statisticSettings = configuration.GetSection(nameof(StatisticSettings)).Get<StatisticSettings>()!;
    var services = new ServiceCollection()
           .AddLogging(builder =>
           {
               builder.AddConsole();
           })
           .AddSingleton(client)
           .AddSingleton(commands)
           .AddSingleton(interactions)
           .AddSingleton(statisticSettings)
           .AddSingleton<CommandHandler>()
           .AddSingleton(new DiscordSettings()
           {
               ClientId = discordSettings.ClientId,
               GuildId = discordSettings.GuildId,
               Token = discordSettings.Token,
               Channel = GrpcChannel.ForAddress(statisticSettings.Gateway),
           })
           .BuildServiceProvider();

    client.Log += Logging.LogAsync;
    commands.Log += Logging.LogAsync;
    interactions.Log += Logging.LogAsync;
    client.Ready += ReadyAsync;

    await client.LoginAsync(TokenType.Bot, discordSettings.Token);


    await client.StartAsync();

    await services.GetRequiredService<CommandHandler>().InitializeAsync();

    while (client.ConnectionState != Discord.ConnectionState.Connected) { }

    await client.SetActivityAsync(new Game($"{environment.EnvironmentName}..."
        , ActivityType.Playing
        , ActivityProperties.Play
        , "no comments"));

    await ReadyAsync();
    async Task ReadyAsync()
    {
        try
        {
            //// Now that we have our builder, we can call the CreateApplicationCommandAsync method to make our slash command.
            //await guild.CreateApplicationCommandAsync(guildCommand.Build());

            //// With global commands we don't need the guild.
            //// await client.CreateGlobalApplicationCommandAsync(globalCommand.Build());
            //// Using the ready event is a simple implementation for the sake of the example. Suitable for testing and development.
            //// For a production bot, it is recommended to only run the CreateGlobalApplicationCommandAsync() once for each command.
            ///

            if (!environment.IsProduction())
            {
                await client.SetStatusAsync(UserStatus.Idle);

                await interactions.RegisterCommandsToGuildAsync(discordSettings.GuildId);
            }
            else
            {
                // this method will add commands globally, but can take around an hour
                //await interactions.RegisterCommandsGloballyAsync();
                await interactions.RegisterCommandsToGuildAsync(discordSettings.GuildId);

            }
        }
        catch (HttpException exception)
        {
            // If our command was invalid, we should catch an ApplicationCommandException.
            // This exception contains the path of the error as well as the error message.
            // You can serialize the Error field in the exception to get a visual of where your error is.
            var json = JsonSerializer.Serialize(exception.Errors);

            // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
            Console.WriteLine(json);
        }

        Console.WriteLine($"Connected as -> [{client.CurrentUser}] :)");
    }
}

var app = builder.Build();

await StartAsync();

await app.RunAsync();

