using AutoMapper;
using Microsoft.Extensions.Logging;

namespace GenshinImpact.Services.Services;

public class GachaHistoryService(IRepository<GachaHistory, long> repository
    , IMapper mapper
    , ILogger<GachaHistoryService> logger) : IGachaHistoryService
{
    private readonly IRepository<GachaHistory, long> _repository = repository;
    private readonly ILogger _logger = logger;
    private readonly IMapper _mapper = mapper;

    public async IAsyncEnumerable<long> CrawlAsync(string authKey)
    {
        _logger.LogInformation("Start: crawl {authKey}", authKey);

        using var client = new HttpClient();

        long endId = 0;
        var total = 0;
        var batchSize = 5000;
        var hasMoreRecords = true;
        var records = new List<GachaHistory>();
        while (hasMoreRecords)
        {
            var items = await GetRecordsAsync(client, endId, authKey);

            switch (items.Count)
            {
                case > 0:
                    records.AddRange(items);
                    endId = items[items.Count - 1].Id;
                    if (records.Count > batchSize)
                    {
                        await _repository.BulkInsertAsync(records);
                        records.Clear();
                    }
                    break;
                default:
                    hasMoreRecords = false;
                    break;
            }

            total += items.Count;

            await Task.Delay(500);

            yield return total;
        }

        if (records.Count > 0)
        {
            await _repository.BulkInsertAsync(records);
        }

        yield return total;

        _logger.LogInformation("End: crawl success {total}", total);
    }

    public async Task<GachaHistoryResponse> FindByIdAsync(long id)
    {
        var result = await _repository.FindByIdAsync(id);
        return _mapper.Map<GachaHistoryResponse>(result);
    }

    private async Task<List<GachaHistory>> GetRecordsAsync(HttpClient client, long endId, string authKey)
    {
        var url = "https://hk4e-api-os.hoyoverse.com/gacha_info/api/getGachaLog";
        var queryParams = new Dictionary<string, string>
        {
            { "win_mode", "fullscreen" },
            { "authkey_ver", "1" },
            { "sign_type", "2" },
            { "auth_appid", "webview_gacha" },
            { "init_type", "301" },
            { "gacha_id", "85107f12e41dbe89924f52b0c0891544d6d32bd2" },
            { "timestamp", "1699400296" },
            { "lang", "vi" },
            { "device_type", "pc" },
            { "game_version", "OSRELWin4.2.0_R19421453_S19245959_D19561600" },
            { "region", "os_asia" },
            { "game_biz", "hk4e_global" },
            { "gacha_type", "301" },
            { "page", "1" },
            { "size", "20" },
            { "end_id", $"{endId}" },
            { "authkey", authKey },
        }.Select(kvp => $"{kvp.Key}={kvp.Value}");

        var requestUri = $"{url}?{string.Join("&", queryParams)}";

        var response = await client.GetAsync(requestUri).ConfigureAwait(false);

        var stream = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<GachaInfoResponse>(stream);
        if (result?.Code == GI.Code.VisitTooFrequently)
        {
            await GetRecordsAsync(client, endId, authKey);
        }

        return result?.Data.GachaHistories.Select(_mapper.Map<GachaHistory>).ToList() ?? [];
    }
}
