using MongoDB.Bson.Serialization.Attributes;

namespace Models.AggregateModels
{
    [BsonIgnoreExtraElements]
    public class AggregateGachaHistoryModel
    {
        [BsonElement("_id")]
        public long Id { get; set; } = default!;
        public int PullIndex { get; set; } = default!;
        public string Name { get; set; } = default!;
    }
}
