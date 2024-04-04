using Common.Converters;

namespace Random.ExternalService.Models.Response;

public class GachaHistoryItemResponse
{
    [JsonPropertyName("uid")]
    public string Uid { get; set; } = default!;

    [JsonPropertyName("gacha_type")]
    public string GachaType { get; set; } = default!;

    [JsonPropertyName("item_id")]
    public string ItemId { get; set; } = default!;

    [JsonPropertyName("count")]
    [JsonConverter(typeof(JsonTypeConverter<int>))]
    public int Count { get; set; }

    [JsonPropertyName("time")]
    public string Time { get; set; } = default!;

    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [JsonPropertyName("lang")]
    public string Lang { get; set; } = default!;

    [JsonPropertyName("item_type")]
    public string ItemType { get; set; } = default!;

    [JsonPropertyName("rank_type")]
    public string RankType { get; set; } = default!;

    [JsonPropertyName("id")]
    [JsonConverter(typeof(JsonTypeConverter<long>))]
    public long Id { get; set; }

    public string TimeZone { get; set; } = "UTC";
}
