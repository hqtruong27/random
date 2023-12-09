using Common.Enum.Hoyoverse;
using System.Text.Json.Serialization;

namespace Models.GenshinImpact;

public class GachaInfoResponse
{
    [JsonPropertyName("retcode")]
    public int RetCode { get; set; }
    [JsonPropertyName("message")]
    public string Message { get; set; }
    [JsonPropertyName("data")]
    public GachaInfoDataResponse Data { get; set; }
}

public class GachaInfoDataResponse
{
    [JsonPropertyName("page")]
    public string Page { get; set; }
    [JsonPropertyName("size")]
    public string Size { get; set; }
    [JsonPropertyName("total")]
    public string Total { get; set; }
    [JsonPropertyName("list")]
    public List<GachaHistoryResponse> GachaHistorys { get; set; }
}

public class GachaHistoryResponse
{
    private string _gachaType = string.Empty;
    private string _rankType = string.Empty;

    [JsonPropertyName("uid")]
    public string Uid { get; set; }

    [JsonPropertyName("gacha_type")]
    public string GachaType { get; set; }

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
    public string RankType { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }
}