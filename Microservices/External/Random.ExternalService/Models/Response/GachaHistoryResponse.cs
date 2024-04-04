namespace Random.ExternalService.Models.Response;

public class GachaHistoryResponse
{
    [JsonPropertyName("retcode")]
    public int Code { get; set; }
    [JsonPropertyName("message")]
    public string Message { get; set; } = default!;
    [JsonPropertyName("data")]
    public GachaHistoryDataResponse Data { get; set; } = default!;
}
