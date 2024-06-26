﻿namespace Random.ExternalService.Models.Request;

public class GetGachaHistoryRequest
{
    [AliasAs("win_mode")]
    [JsonPropertyName("win_mode")]
    public string WinMode { get; set; } = default!;

    [AliasAs("authkey_ver")]
    [JsonPropertyName("authkey_ver")]
    public int AuthKeyVer { get; set; }

    [AliasAs("sign_type")]
    [JsonPropertyName("sign_type")]
    public int SignType { get; set; }

    [AliasAs("auth_appid")]
    [JsonPropertyName("auth_appid")]
    public string AuthAppId { get; set; } = default!;

    [AliasAs("init_type")]
    [JsonPropertyName("init_type")]
    public int InitType { get; set; }

    [AliasAs("gacha_id")]
    [JsonPropertyName("gacha_id")]
    public string GachaId { get; set; } = default!;

    [AliasAs("timestamp")]
    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; } = default!;

    [AliasAs("lang")]
    [JsonPropertyName("lang")]
    public string Lang { get; set; } = default!;

    [AliasAs("device_type")]
    [JsonPropertyName("device_type")]
    public string DeviceType { get; set; } = default!;

    [AliasAs("game_version")]
    [JsonPropertyName("game_version")]
    public string GameVersion { get; set; } = default!;

    [AliasAs("region")]
    [JsonPropertyName("region")]
    public string Region { get; set; } = default!;

    [AliasAs("game_biz")]
    [JsonPropertyName("game_biz")]
    public string GameBiz { get; set; } = default!;

    [AliasAs("gacha_type")]
    [JsonPropertyName("gacha_type")]
    public int GachaType { get; set; }

    [AliasAs("page")]
    [JsonPropertyName("page")]
    public int Page { get; set; }

    [AliasAs("size")]
    [JsonPropertyName("size")]
    public int Size { get; set; }

    [AliasAs("end_id")]
    [JsonPropertyName("end_id")]
    public long EndId { get; set; }

    [AliasAs("begin_id")]
    [JsonPropertyName("begin_id")]
    public long BeginId { get; set; }

    [AliasAs("authkey")]
    [JsonPropertyName("authkey")]
    public string AuthKey { get; set; } = default!;

    public void Update(int gachaType, long beginId, long endId, int size = 20)
    {
        GachaType = gachaType; EndId = 0; BeginId = 0;
        if (endId > 0)
        {
            EndId = endId;
        }

        if (beginId > 0)
        {
            BeginId = beginId;
        }

        Size = size;
    }

    public string ToUri(int gachaType, long beginId, long endId, int size = 20)
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

        string queryStringUrl = "?" + string.Join(
            "&",
            queryParams
                .AllKeys.Select(
                  key => $"{HttpUtility.UrlEncode(key)}={HttpUtility.UrlEncode(queryParams[key])}"));

        return queryStringUrl;
    }
}