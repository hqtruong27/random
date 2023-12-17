using Common.Enum;
using Discord.Interactions;
using Microsoft.Extensions.Logging;

namespace Discord.Bot.Services.Interactions;

[Group(name: "spending", description: "This is spending")]
public class Spending(DiscordSettings discord, ILogger<Spending> logger) : InteractionModuleBase<SocketInteractionContext>
{
    private readonly SpendingClient _spendingClient = new(discord.Channel);
    private readonly ILogger _logger = logger;

    [SlashCommand("create", "create a spending")]
    public async Task Create(
          [Summary(description: "Enter a name for the spending")] string name
        , [Summary(description: "Enter the amount spent this time, ex 10000 -> 10.000đ")] ulong amount
        , [Summary(description: "Enter the purpose spent this time")] SpendingPurpose purpose
        , [Summary(description: "Notes on this spending report")] string description)
    {
        _logger.LogInformation("Start: create spending...");

        _ = Task.Run(async () =>
        {
            var user = Context.User.ToSocketGuild();
            var response = await _spendingClient.CreateAsync(new()
            {
                UserId = user.Id.ToString(),
                Name = name,
                Amount = (long)amount,
                Description = description,
                Status = SpendingStatus.Active.ToString(),
                Purpose = purpose.ToString(),
            });

            await Context.Channel.SendMessageAsync($"{user.Mention} {response.Description}");

            await Context.User.SendMessageAsync($"Spending created: {amount} {description} {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            _logger.LogInformation("End: create success");
        });

        await RespondAsync("processing...");
    }

    [SlashCommand("list", "list spending")]
    public async Task List()
    {
        _logger.LogInformation("Start: list spending...");

        _ = Task.Run(async () =>
        {
            var user = Context.User.ToSocketGuild();
            var response = await _spendingClient.GetSpendingsByUserIdAsync(new GetSpendingsByUserIdRequest
            {
                UserId = user.Id()
            });

            // Determine the maximum length of each header
            var expenses = response.Items;
            // Calculate the header lengths based on values and headers
            var headerLengths = new Dictionary<string, int>();
            foreach (var expense in expenses)
            {
                UpdateHeaderLength(headerLengths, "Name", expense.Name.Length);
                UpdateHeaderLength(headerLengths, "Amount", expense.Amount.ToString().Length);
                UpdateHeaderLength(headerLengths, "Description", Math.Min(expense.Description.Length, 20));
                UpdateHeaderLength(headerLengths, "Purpose", expense.Purpose.Length);
            }

            // Print the headers
            var message = $"{("Name".PadRight(headerLengths["Name"]))}  {("Amount".PadRight(headerLengths["Amount"]))}  {("Description".PadRight(headerLengths["Description"]))}  {("Purpose".PadRight(headerLengths["Purpose"]))}\n";

            // Print the expense details
            foreach (var expense in expenses)
            {
                string description = expense.Description.Length > 20 ? $"{expense.Description.Substring(0, 17)}..." : expense.Description;
                message += $"{expense.Name.PadRight(headerLengths["Name"])}  {expense.Amount.ToString().PadRight(headerLengths["Amount"])}  {description.PadRight(headerLengths["Description"])}  {expense.Purpose.PadRight(headerLengths["Purpose"])}\n";
            }

            Console.WriteLine(message);
            await Context.Channel.SendMessageAsync($"```{message}```");

            _logger.LogInformation("End: list spending success");
        });

        await RespondAsync("processing...");
    }

    [SlashCommand("health-check", "check connect to server spending")]
    public async Task HealthCheck()
    {
        _ = Task.Run(async () =>
         {
             var check = await _spendingClient.HealthCheckAsync(new());
             await Context.Channel.SendMessageAsync("Live!!");
         });

        await RespondAsync("checking....");
    }

    public override Task AfterExecuteAsync(ICommandInfo command)
    {
        return Task.CompletedTask;
    }

    private static void UpdateHeaderLength(Dictionary<string, int> headerLengths, string header, int valueLength)
    {
        int headerLength = header.Length;
        int maxLength = Math.Max(headerLength, valueLength);

        if (!headerLengths.TryGetValue(header, out int value) || maxLength > value)
        {
            value = maxLength;
            headerLengths[header] = value;
        }
    }
}