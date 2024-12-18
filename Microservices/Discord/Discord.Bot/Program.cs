var builder = Host.CreateApplicationBuilder(args);

var (services, configuration, assembly) = await builder.SetupAsync();

var discord = services.AddDiscordClient(configuration, builder.Environment, assembly);

await discord.UseLavalink(configuration).ConnectAsync();

await discord.ConnectAsync();

await discord.SetStartUpStatusAsync();

await builder.Build().RunAsync();