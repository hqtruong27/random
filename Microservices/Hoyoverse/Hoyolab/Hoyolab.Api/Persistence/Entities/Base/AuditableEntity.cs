﻿namespace Hoyolab.Api.Persistence.Entities.Base;

public class AuditableEntity : IAuditableEntity<string>
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
}

public class AuditableEntity<T> : IAuditableEntity<T>
{
    [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
    public T Id { get; set; } = default!;
    public T? CreatedBy { get; set; }
    public DateTime? Created { get; set; }
    public T? UpdatedBy { get; set; }
    public DateTime? Updated { get; set; }
}