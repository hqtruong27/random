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

var app = builder.Build();

await app.RunAsync();