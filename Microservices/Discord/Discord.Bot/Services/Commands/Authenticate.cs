using Discord.Commands;

namespace Discord.Bot.Services.Commands;

//public class Spending(DiscordSettings discord) : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>

public class Authenticate(DiscordSettings discord) : ModuleBase<SocketCommandContext>
{
    private readonly UserClient _userClient = new(discord.Channel);

    [Command("init")]
    public async Task InitAsync()
    {
        var user = Context.User;
        var discriminator = user.DiscriminatorValue;

        var response = await _userClient.CreateAsync(new CreateUserRequest
        {
            Id = user.Id.ToString(),
            Name = user.GlobalName,
            GlobalName = user.GlobalName,
            UserName = discriminator == 0 ? user.Username : $"{user.Username}#{user.DiscriminatorValue}"
        });

        await ReplyAsync(response.Description);
    }

    [Command("login")]
    public async Task AuthenticateAsync()
    {
        await ReplyAsync("Login success!!");
    }
}
