using Infrastructure.Core.Dispatchers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using StarRail.Api.Features.Models;
using StarRail.Core.Entities;
using StarRail.Core.Enums;
using StarRail.Core.Interfaces.Repositories;

namespace StarRail.Api.Features.GachaHistories.Queries
{
    public sealed record WishCalculatorQuery : IQuery<List<WishCounterModel>>
    {
        public class WishCalculatorHandler(IRepository<GachaHistory, ObjectId> repository) : IQueryHandler<WishCalculatorQuery, List<WishCounterModel>>
        {
            public async Task<List<WishCounterModel>> Handle(WishCalculatorQuery request, CancellationToken cancellationToken)
            {
                var result = new List<WishCounterModel>();
                var charLimited = await PityCalculatorAsync(Common.Enum.Hoyoverse.BannerType.Character);
                var weapon = await PityCalculatorAsync(Common.Enum.Hoyoverse.BannerType.Weapon);
                var regular = await PityCalculatorAsync(Common.Enum.Hoyoverse.BannerType.Regular);
                var novice = await PityCalculatorAsync(Common.Enum.Hoyoverse.BannerType.Novice);

                result.Add(charLimited);
                result.Add(weapon);
                result.Add(regular);
                result.Add(novice);

                return result;
            }
            private async Task<WishCounterModel> PityCalculatorAsync(Common.Enum.Hoyoverse.BannerType bannerType)
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

                return new WishCounterModel
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
            private static BsonArray GetGachaTypeCondition(Common.Enum.Hoyoverse.BannerType bannerType) => bannerType switch
            {
                Common.Enum.Hoyoverse.BannerType.Character =>
                [
                    new BsonDocument
                    {
                        {"GachaType", GachaType.CharLimited.ToString() }
                    }
                ],
                Common.Enum.Hoyoverse.BannerType.Weapon =>
                [
                    new BsonDocument
                    {
                        {"GachaType", GachaType.LightCone.ToString() }
                    }
                ],
                Common.Enum.Hoyoverse.BannerType.Regular =>
                [
                    new BsonDocument
                    {
                        {"GachaType", GachaType.Regular.ToString() }
                    }
                ],
                Common.Enum.Hoyoverse.BannerType.Novice =>
                [
                    new BsonDocument
                    {
                        {"GachaType", GachaType.Novice.ToString() }
                    }
                ],
                _ => []
            };
        }
    }
}
