using System.Text.Json.Serialization;

namespace Models.GenshinImpact
{
    public class GachaHistoryQueryString
    {
        [JsonPropertyName("win_mode")]
        public string WinMode { get; set; }

        [JsonPropertyName("authkey_ver")]
        public int AuthKeyVer { get; set; }

        [JsonPropertyName("sign_type")]
        public int SignType { get; set; }

        [JsonPropertyName("auth_appid")]
        public string AuthAppId { get; set; }

        [JsonPropertyName("init_type")]
        public int InitType { get; set; }

        [JsonPropertyName("gacha_id")]
        public string GachaId { get; set; }

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }

        [JsonPropertyName("lang")]
        public string Lang { get; set; }

        [JsonPropertyName("device_type")]
        public string DeviceType { get; set; }

        [JsonPropertyName("game_version")]
        public string GameVersion { get; set; }

        [JsonPropertyName("region")]
        public string Region { get; set; }

        [JsonPropertyName("game_biz")]
        public string GameBiz { get; set; }

        [JsonPropertyName("gacha_type")]
        public int GachaType { get; set; }

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("end_id")]
        public int EndId { get; set; }

        [JsonPropertyName("authkey")]
        public string AuthKey { get; set; }
    }
}
