namespace Infrastructure.Persistence.Schemas.Base;

public class AuditableEntity : IAuditableEntity
{
    [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
    [BsonSerializer(typeof(ObjectIdAsStringSerializer))]
    public string Id { get; set; } = default!;
    [BsonSerializer(typeof(ObjectIdAsStringSerializer))]
    public string? CreatedBy { get; set; }
    [BsonSerializer(typeof(ObjectIdAsStringSerializer))]
    public string? UpdatedBy { get; set; }
    public DateTime? Created { get; set; }
    public DateTime? Updated { get; set; }

    public AuditableEntity()
    {
        Id = ObjectId.GenerateNewId().ToString();
    }
}

public class AuditableEntity<T> : IAuditableEntity<T>
{
    [BsonId(IdGenerator = typeof(ObjectIdGenerator))]
    public T Id { get; set; } = default!;
    public T? CreatedBy { get; set; }
    public DateTime? Created { get; set; }
    public T? UpdatedBy { get; set; }
    public DateTime? Updated { get; set; }

    public AuditableEntity()
    {
        if (typeof(T) == typeof(ObjectId) &&
            (Id == null || EqualityComparer<T>.Default.Equals(Id, default)))
        {
            Id = (T)(object)ObjectId.GenerateNewId();
        }
        else if (typeof(T) == typeof(string) &&
                 (Id == null || EqualityComparer<T>.Default.Equals(Id, default)))
        {
            Id = (T)(object)ObjectId.GenerateNewId().ToString();
        }
    }
}

public class ObjectIdAsStringSerializer : SerializerBase<string>
{
    public override string Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var reader = context.Reader;
        var bsonType = reader.GetCurrentBsonType();
        if (bsonType == BsonType.Null)
        {
            reader.ReadNull();
            return null!;
        }

        if (bsonType == BsonType.ObjectId)
        {
            return reader.ReadObjectId().ToString();
        }

        throw new FormatException($"Cannot deserialize BsonType '{bsonType}' to a string.");
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, string value)
    {
        if (ObjectId.TryParse(value, out var objectId))
        {
            context.Writer.WriteObjectId(objectId);
        }
        else
        {
            context.Writer.WriteNull();
        }
    }
}