var builder = Host.CreateApplicationBuilder(args);

var (services, configuration, assembly) = await builder.SetupAsync();

services.AddOptions(configuration);

var discord = services.AddDiscordClient(configuration, builder.Environment, assembly);

//Registration is only possible after registering for the Discord client;
services.AddInfrastructureCore(configuration); 

var (type, status, name) = Constants.GetRandomShuffledActivity();
await discord.ConnectAsync(new DiscordActivity
{
    Id = name,
    ActivityType = Enum.Parse<ActivityType>(type),
    Name = name
}, Enum.Parse<UserStatus>(status));

await discord.UseLavalink(configuration).ConnectAsync();

new System.Timers.Timer(2 * 60 * 60 * 1000)
{
    AutoReset = true,
    Enabled = true,

}.Elapsed += async (s, e) =>
{
    await discord.SetStartUpStatusAsync();
};

await builder.Build().RunAsync();