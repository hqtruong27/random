using Discord.Bot.Features.Hoyoverse.Hoyolab.Events;

namespace Discord.Bot;

public static class Registration
{
    public static async Task<(IServiceCollection, IConfiguration, Assembly)> SetupAsync(this IHostApplicationBuilder builder)
    {
        var doppler = new DopplerClient();
        var jsonStream = await doppler.FetchSecretsAsStreamAsync();

        var services = builder.Services;
        var environment = builder.Environment;
        var configuration = builder.Configuration
            .SetBasePath(builder.Environment.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)
            .AddJsonStream(jsonStream)
            .AddEnvironmentVariables()
            .Build();

        return (services, configuration, typeof(Program).Assembly);
    }

    public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        var discordOptions = configuration.GetSection(nameof(DiscordOptions));
        var hoyolabOptions = configuration.GetSection(nameof(HoyolabOptions));
        var lavalinkOptions = configuration.GetSection(nameof(LavalinkOptions));

        services.Configure<DiscordOptions>(discordOptions);
        services.Configure<HoyolabOptions>(hoyolabOptions);
        services.Configure<LavalinkOptions>(lavalinkOptions);

        services.AddSingleton(discordOptions.Get<DiscordOptions>()!);
        services.AddSingleton(hoyolabOptions.Get<HoyolabOptions>()!);
        services.AddSingleton(lavalinkOptions.Get<LavalinkOptions>()!);

        return services;
    }

    public static IServiceCollection AddInfrastructureCore(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDomainEventPublisher()
            .AddEventPublisher(builder =>
            {
                builder.RegisterEventHandler<RedeemCodeRedeemed, RedeemCodeRedeemedHandler>();
                builder.UseAwsSnsSqs(configure => configuration.GetSection("Aws").Bind(configure));
            });

        return services;
    }

    public static DiscordClient AddDiscordClient(this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment,
        Assembly assembly
        )
    {
        var discordOptions = configuration.GetSection(nameof(DiscordOptions)).Get<DiscordOptions>()!;

        var discord = new DiscordClient(new DiscordConfiguration
        {
            Token = discordOptions.Token,
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

        services.AddSingleton(discord);

        var commands = discord.UseApplicationCommands();
        if (environment.IsProduction())
        {
            commands.RegisterGlobalCommands(assembly);
        }
        else
        {
            commands.RegisterGuildCommands(assembly, discordOptions.GuildId);
        }

        discord.UseCommandsNext(new()
        {
            StringPrefixes = ["!", "~", "-"],
            ServiceProvider = services.BuildServiceProvider(),
        }).RegisterCommands(assembly);

        return discord;
    }

    public static (LavalinkExtension, LavalinkConfiguration) UseLavalink(this DiscordClient discord, IConfiguration configuration)
    {
        var options = configuration.GetSection(nameof(LavalinkOptions)).Get<LavalinkOptions>()!;
        ConnectionEndpoint endpoint = new()
        {
            Hostname = options.Host,
            Port = options.Port
        };

        return (discord.UseLavalink(), new()
        {
            Password = options.Password,
            RestEndpoint = endpoint,
            SocketEndpoint = endpoint,
            EnableBuiltInQueueSystem = true
        });
    }

    public static async Task<LavalinkSession?> ConnectAsync(this (LavalinkExtension Extension, LavalinkConfiguration Configuration) lavalink)
    {
        try
        {
            return await lavalink.Extension.ConnectAsync(lavalink.Configuration);
        }
        catch
        {
            // ignored
        }

        return null;
    }

    public static async Task SetStartUpStatusAsync(this DiscordClient discord)
    {
        try
        {
            var (type, status, name) = Constants.GetRandomShuffledActivity();

            await discord.UpdateStatusAsync(new DiscordActivity
            {
                Id = name,
                ActivityType = Enum.Parse<ActivityType>(type),
                Name = name
            }, Enum.Parse<UserStatus>(status));
        }
        catch
        {
            
        }
    }
}
