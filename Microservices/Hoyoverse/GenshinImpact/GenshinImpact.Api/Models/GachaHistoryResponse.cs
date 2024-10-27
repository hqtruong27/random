using GenshinImpact.Core.EnumTypes;
using GenshinImpact.Shared.Converters;
using System.Text.Json.Serialization;

namespace GenshinImpact.Api.Models;

public class GachaInfoResponse
{
    [JsonPropertyName("retcode")]
    public int Code { get; set; }
    [JsonPropertyName("message")]
    public string Message { get; set; } = default!;
    [JsonPropertyName("data")]
    public GachaInfoDataResponse Data { get; set; } = default!;
}

public class GachaInfoDataResponse
{
    [JsonPropertyName("page")]
    public string Page { get; set; } = default!;
    [JsonPropertyName("size")]
    public string Size { get; set; } = default!;
    [JsonPropertyName("total")] 
    public string Total { get; set; } = default!;
    [JsonPropertyName("list")]
    public List<GachaHistoryResponse> GachaHistories { get; set; } = default!;
}

public class GachaHistoryResponse
{
    [JsonPropertyName("uid")]
    public string Uid { get; set; } = default!;

    [JsonPropertyName("gacha_type")]
    [JsonConverter(typeof(JsonTypeConverter<GachaType>))]
    public GachaType GachaType { get; set; }

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
    [JsonConverter(typeof(JsonTypeConverter<RankType>))]
    public RankType RankType { get; set; }

    [JsonPropertyName("id")]
    [JsonConverter(typeof(JsonTypeConverter<long>))]
    public long Id { get; set; }

    public string TimeZone { get; set; } = "UTC";
}