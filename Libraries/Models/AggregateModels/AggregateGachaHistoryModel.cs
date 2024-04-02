using MongoDB.Bson.Serialization.Attributes;

namespace Models.AggregateModels
{
    [BsonIgnoreExtraElements]
    public class AggregateGachaHistoryModel
    {
        public long ReferenceId { get; set; }
        public int PullIndex { get; set; }
        public string Name { get; set; } = default!;
    }
}
