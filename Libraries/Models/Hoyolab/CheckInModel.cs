using System.Text.Json.Serialization;

namespace Models.Hoyolab;

public class CheckInRequest
{
    public required string DiscordId { get; set; }
}


public class CheckInResponse
{
    [JsonPropertyName("retcode")]
    public int Code { get; set; }
    [JsonPropertyName("message")]
    public string Message { get; set; } = default!;
    [JsonIgnore]
    public int Type { get; set; }
}
