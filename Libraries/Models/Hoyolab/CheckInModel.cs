using System.Text.Json.Serialization;

namespace Models.Hoyolab;

public class CheckInRequest
{
    public string DiscordId { get; set; } = default!;

    [JsonPropertyName("act_id")]
    public string ActId { get; set; } = default!;
}


public class CheckInResponse
{
    [JsonPropertyName("retcode")]
    public int Code { get; set; }
    [JsonPropertyName("message")]
    public string Message { get; set; } = default!;
}
