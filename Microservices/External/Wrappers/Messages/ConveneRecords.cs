namespace Wrappers.Messages;

public class ConveneRecordsRequest
{
    [JsonPropertyName("playerId")]
    public string PlayerId { get; set; } = default!;

    [JsonPropertyName("cardPoolId")]
    public string CardPoolId { get; set; } = default!;

    [JsonPropertyName("cardPoolType")]
    public int CardPoolType { get; set; }

    [JsonPropertyName("serverId")]
    public string ServerId { get; set; } = default!;

    [JsonPropertyName("languageCode")]
    public string LanguageCode { get; set; } = default!;

    [JsonPropertyName("recordId")]
    public string RecordId { get; set; } = default!;
}
