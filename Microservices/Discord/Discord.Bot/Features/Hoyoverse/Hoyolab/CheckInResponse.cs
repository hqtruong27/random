namespace Discord.Bot.Features.Hoyoverse.Hoyolab;

public class CheckInResponse
{
    [JsonPropertyName("retcode")]
    public int Code { get; set; }
    [JsonPropertyName("message")]
    public string Message { get; set; } = default!;
    [JsonIgnore]
    public int Type { get; set; }
    public string Name { get; set; } = default!;
}
