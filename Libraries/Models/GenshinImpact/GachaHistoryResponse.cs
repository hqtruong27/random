using Common.Enum.Hoyoverse;
using System.Text.Json.Serialization;

namespace Models.GenshinImpact;

public class GachaHistoryResponse
{
    private string _gachaType = string.Empty;
    private string _rankType = string.Empty;

    [JsonPropertyName("uid")]
    public string Uid { get; set; }

    [JsonPropertyName("gacha_type")]
    public GachaType GachaType { get => (GachaType)int.Parse(_gachaType); set => _gachaType = ((int)value).ToString(); }

    [JsonPropertyName("item_id")]
    public string ItemId { get; set; }

    [JsonPropertyName("count")]
    public string Count { get; set; }

    [JsonPropertyName("time")]
    public string Time { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("lang")]
    public string Lang { get; set; }

    [JsonPropertyName("item_type")]
    public string ItemType { get; set; }

    [JsonPropertyName("rank_type")]
    public RankType RankType { get => (RankType)int.Parse(_rankType); set => _rankType = ((int)value).ToString(); }

    [JsonPropertyName("id")]
    public string Id { get; set; }
}