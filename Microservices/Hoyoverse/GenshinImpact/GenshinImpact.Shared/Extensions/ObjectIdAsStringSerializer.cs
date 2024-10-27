using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace GenshinImpact.Shared.Extensions;

public class ObjectIdAsStringSerializer : SerializerBase<string>
{
    public override string Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        return context.Reader.ReadObjectId().ToString();
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, string value)
    {
        context.Writer.WriteObjectId(ObjectId.Parse(value));
    }
}