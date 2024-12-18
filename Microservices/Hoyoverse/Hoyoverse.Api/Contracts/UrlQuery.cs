using System.Collections.Specialized;
using System.Web;

namespace Hoyoverse.Contracts;

public class UrlQuery
{
    [JsonPropertyName("win_mode")]
    public string WinMode { get; set; } = default!;

    [JsonPropertyName("authkey_ver")]
    public int AuthKeyVer { get; set; }

    [JsonPropertyName("sign_type")]
    public int SignType { get; set; }

    [JsonPropertyName("auth_appid")]
    public string AuthAppId { get; set; } = default!;

    [JsonPropertyName("init_type")]
    public int InitType { get; set; }

    [JsonPropertyName("gacha_id")]
    public string GachaId { get; set; } = default!;

    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; } = default!;

    [JsonPropertyName("lang")]
    public string Lang { get; set; } = default!;

    [JsonPropertyName("device_type")]
    public string DeviceType { get; set; } = default!;

    [JsonPropertyName("game_version")]
    public string GameVersion { get; set; } = default!;

    [JsonPropertyName("region")]
    public string Region { get; set; } = default!;

    [JsonPropertyName("game_biz")]
    public string GameBiz { get; set; } = default!;

    [JsonPropertyName("gacha_type")]
    public int GachaType { get; set; }

    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("size")]
    public int Size { get; set; }

    [JsonPropertyName("end_id")]
    public int EndId { get; set; }

    [JsonPropertyName("authkey")]
    public string AuthKey { get; set; } = default!;

    public string ToQueryParams(int gachaType, long beginId, long endId, int size = 20)
    {
        var queryParams = new NameValueCollection
        {
            { "win_mode", WinMode },
            { "authkey_ver", AuthKeyVer.ToString() },
            { "sign_type", SignType.ToString() },
            { "auth_appid", AuthAppId },
            { "init_type", InitType.ToString() },
            { "gacha_id", GachaId },
            { "timestamp", Timestamp },
            { "lang", Lang },
            { "device_type", DeviceType },
            { "game_version", GameVersion },
            { "region", Region },
            { "game_biz", GameBiz },
            { "gacha_type", $"{gachaType}" },
            { "page", Page.ToString() },
            { "size", size.ToString() },
            { "authkey", AuthKey }
        };

        if (beginId > 0)
        {
            queryParams.Add(new()
            {
                { "begin_id", beginId.ToString() },
            });
        }

        if (endId > 0 || beginId == 0)
        {
            queryParams.Add(new()
            {
                { "end_id", endId.ToString() },
            });
        }

        var queryStringUrl = "?" + string.Join(
            "&",
            queryParams
                .AllKeys.Select(
                    key => $"{HttpUtility.UrlEncode(key)}={HttpUtility.UrlEncode(queryParams[key])}"));

        return queryStringUrl;
    }
}