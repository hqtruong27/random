using AutoMapper;
using Common.Enum.Hoyoverse;
using Common.Helpers;
using Microsoft.Extensions.Logging;
using Models.AggregateModels;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace GenshinImpact.Services.Services;

public class GachaHistoryService(IRepository<GachaHistory, long> gachaHistoryRepository
    , ISettingsRepository settingsRepository
    , IMapper mapper
    , ILogger<GachaHistoryService> logger) : IGachaHistoryService
{
    private readonly IRepository<GachaHistory, long> _gachaHistoryRepository = gachaHistoryRepository;
    private readonly ISettingsRepository _settingsRepository = settingsRepository;
    private readonly ILogger _logger = logger;
    private readonly IMapper _mapper = mapper;

    public async IAsyncEnumerable<long> CrawlAsync(string url)
    {
        _logger.LogInformation("Start: crawl {url}", url);

        var queryString = QueryStringHelper.Populate<GachaHistoryQueryString>(url);
        var config = await _settingsRepository.GetSettingsAsync<WishListConfig>("WISH_LIST_CONFIG");
        using var client = new HttpClient();
        long endId = 0;
        var total = 0;
        var batchSize = 5000;
        var hasMoreRecords = true;
        var records = new List<GachaHistory>();
        while (hasMoreRecords)
        {
            var items = await GetRecordsAsync(client, endId, config.GachaUrl, queryString);

            switch (items.Count)
            {
                case > 0:
                    records.AddRange(items);
                    endId = items[items.Count - 1].Id;
                    if (records.Count > batchSize)
                    {
                        await _gachaHistoryRepository.BulkInsertAsync(records);
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
            await _gachaHistoryRepository.BulkInsertAsync(records);
        }

        yield return total;

        _logger.LogInformation("End: crawl success {total}", total);
    }

    public async Task<GachaHistoryResponse> FindByIdAsync(long id)
    {
        var result = await _gachaHistoryRepository.FindByIdAsync(id);
        return _mapper.Map<GachaHistoryResponse>(result);
    }

    public async IAsyncEnumerable<WishCounterModel> WishCalculatorAsync()
    {
        var result = new List<WishCounterModel>();
        var charLimited = await PityCalculatorAsync(BannerType.Character);
        var weapon = await PityCalculatorAsync(BannerType.Weapon);
        var regular = await PityCalculatorAsync(BannerType.Regular);

        result.Add(charLimited);
        result.Add(weapon);
        result.Add(regular);

        foreach (var item in result)
        {
            yield return item;
        }
    }

    private async Task<WishCounterModel> PityCalculatorAsync(BannerType bannerType)
    {
        var gachaType = GetGachaTypeCondition(bannerType);
        var stage1 = new BsonDocument
        {
            {
                "$match", new BsonDocument
                {
                    {
                        "$or", gachaType
                    }
                }
            }
        };
        var stage2 = new BsonDocument
        {
            {
                "$setWindowFields", new BsonDocument
                {
                    { "partitionBy", "_id" },
                    { "sortBy", new BsonDocument(new BsonElement( "_id", 1 ))},
                    { "output", new BsonDocument
                        {
                            { "PullIndex", new BsonDocument
                                {
                                    {"$documentNumber",  new BsonDocument() }
                                }
                            }
                        }
                    }
                }
            }
        };
        var stage3 = new BsonDocument
        {
            {
                "$match", new BsonDocument
                {
                    { "RankType", RankType.Five.ToString() },
                }
            }
        };
        var countStage = new BsonDocument
        {
            {
                "$count", "Total"
            }
        };

        var characterEvent = await _gachaHistoryRepository.AggregateAsync(stage1, stage2, stage3);
        var count = (await _gachaHistoryRepository.AggregateAsync(stage1, countStage)).FirstOrDefault()?.GetValue("Total").AsInt32;
        var aggregateModel = BsonSerializer.Deserialize<List<AggregateGachaHistoryModel>>(characterEvent.ToJson());

        AggregateGachaHistoryModel? first = null;
        var listEvent = new List<Event>();

        foreach (var item in aggregateModel)
        {
            if (first == null)
            {
                first = item;
                listEvent.Add(new Event
                {
                    ItemName = item.Name,
                    PullIndex = item.PullIndex
                });
                continue;
            }
            listEvent.Add(new Event
            {
                ItemName = item.Name,
                PullIndex = item.PullIndex - first.PullIndex
            });
            first = item;
        }

        return new WishCounterModel
        {
            Banner = bannerType,
            Detail = new WishBanner
            {
                Events = listEvent,
                Pity = (count - first?.PullIndex ?? 0),
                TotalPulls = count ?? 0
            }
        };
    }

    private static BsonArray GetGachaTypeCondition(BannerType bannerType) => bannerType switch
    {
        BannerType.Character =>
        [
            new BsonDocument
            {
                {"GachaType", GachaType.CharLimited.ToString() }
            },
            new BsonDocument
            {
                {"GachaType", "400" }
            }
        ],
        BannerType.Weapon =>
        [
            new BsonDocument
            {
                {"GachaType", GachaType.Weapons.ToString() }
            }
        ],
        BannerType.Regular =>
        [
            new BsonDocument
            {
                {"GachaType", GachaType.Regular.ToString() }
            }
        ],
        BannerType.Novice =>
       [
           new BsonDocument
            {
                {"GachaType", GachaType.Regular.ToString() }
            }
       ],
        _ => []
    };

    private async Task<List<GachaHistory>> GetRecordsAsync(
        HttpClient client,
        long endId,
        string gachaUrl,
        GachaHistoryQueryString gachaHistoryQueryString)
    {
        var requestUri = $"{gachaUrl}?{gachaHistoryQueryString.QueryString}";

        var response = await client.GetAsync(requestUri).ConfigureAwait(false);

        var stream = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<GachaInfoResponse>(stream);
        if (result?.Code == GI.Code.AuthKeyTimeOut)
        {
            throw new Exception("Authkey timeout");
        }

        if (result?.Code == GI.Code.VisitTooFrequently)
        {
            await GetRecordsAsync(client, endId, gachaUrl, gachaHistoryQueryString);
        }

        return result?.Data.GachaHistories.Select(_mapper.Map<GachaHistory>).ToList() ?? [];
    }
}
