namespace Random.ExternalService.Models.Response;

public class GachaHistoryDataResponse
{
    [JsonPropertyName("page")]
    public string Page { get; set; } = default!;
    [JsonPropertyName("size")]
    public string Size { get; set; } = default!;
    [JsonPropertyName("total")]
    public string Total { get; set; } = default!;
    [JsonPropertyName("list")]
    public List<GachaHistoryItemResponse> Items { get; set; } = default!;
}
