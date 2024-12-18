﻿namespace Infrastructure.Persistence.Schemas.Base;

public class Entity<T> : IEntity<T>
{
    public T Id { get; set; } = default!;
    public bool IsTransient() => Id != null && Id.Equals(default(T));
}