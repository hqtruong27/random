using Discord.Interactions;
using Grpc.Net.Client;
using StarRail.Api.Protos;

namespace Discord.Bot.Services.Interactions;

[Group(name: "star-rail", description: "Star Rail group")]
public class StarRail : InteractionModuleBase<SocketInteractionContext>
{
    private readonly StarRailService.StarRailServiceClient _client = new(GrpcChannel.ForAddress("https://localhost:7041"));

    [SlashCommand("wish-history", "check wish history")]
    public async Task WishHistoryAsync()
    {
        _ = Task.Run(async () =>
        {
            var response = await _client.WishHistoryAsync(new());
            var wish = response.WishHistories;

            await Context.Channel.SendMessageAsync("Ok!");
        });

        await RespondAsync("processing...");
    }
}
