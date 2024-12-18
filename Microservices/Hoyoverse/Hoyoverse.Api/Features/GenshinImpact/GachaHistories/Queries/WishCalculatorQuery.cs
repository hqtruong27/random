﻿namespace Hoyoverse.Features.GenshinImpact.GachaHistories.Queries;

[Tags("GenshinImpact")]
[Get("genshin-impact/wish-calculator")]
public sealed record WishCalculatorQuery : IRequest<List<WishCounterModel>>;

public class WishCalculatorHandler(IRepository<GenshinImpactGachaHistory> repository)
    : IRequestHandler<WishCalculatorQuery, List<WishCounterModel>>
{
    public async Task<List<WishCounterModel>> Handle(WishCalculatorQuery request, CancellationToken cancellationToken)
    {
        var charLimited = await PityCalculatorAsync(BannerType.Character, cancellationToken);
        var weapon = await PityCalculatorAsync(BannerType.Weapon, cancellationToken);
        var regular = await PityCalculatorAsync(BannerType.Regular, cancellationToken);

        return [charLimited, weapon, regular];
    }
    private async Task<WishCounterModel> PityCalculatorAsync(BannerType bannerType, CancellationToken cancellationToken)
    {
        var gachaType = GetGachaTypeCondition(bannerType);
        var stage1 = new BsonDocument
        {
            {
                "$match",
                new BsonDocument
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
                "$setWindowFields",
                new BsonDocument
                {
                    { "partitionBy", "ReferenceId" },
                    { "sortBy", new BsonDocument(new BsonElement( "ReferenceId", 1 ))},
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
                "$match",
                new BsonDocument
                {
                    { "RankType", nameof(GenshinImpactRankType.Five) },
                }
            }
        };

        var countStage = new BsonDocument
        {
            {
                "$count", "Total"
            }
        };

        var characterEvent = await repository.AggregateAsync(stage1, stage2, stage3);
        var count = (await repository.AggregateAsync(stage1, countStage)).FirstOrDefault()?.GetValue("Total").AsInt32;
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

        return new()
        {
            Banner = bannerType,
            Detail = new WishBanner
            {
                Events = listEvent,
                Pity = count - first?.PullIndex ?? 0,
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
                { "GachaType", nameof(GenshinImpactGachaType.CharLimited) }
            },
            new BsonDocument
            {
                { "GachaType", nameof(GenshinImpactGachaType.CharLimitedTwo) }
            }
        ],
        BannerType.Weapon =>
        [
            new BsonDocument
            {
                { "GachaType", nameof(GenshinImpactGachaType.Weapons) }
            }
        ],
        BannerType.Regular =>
        [
            new BsonDocument
            {
                { "GachaType", nameof(GenshinImpactGachaType.Regular) }
            }
        ],
        BannerType.Novice =>
        [
            new BsonDocument
            {
                { "GachaType", nameof(GenshinImpactGachaType.Novice) }
            }
        ],
        _ => []
    };
}