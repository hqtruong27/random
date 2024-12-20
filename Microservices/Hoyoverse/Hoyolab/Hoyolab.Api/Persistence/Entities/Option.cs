﻿using Hoyolab.Api.Persistence.Entities.Base;

namespace Hoyolab.Api.Persistence.Entities;

public class Option : AuditableEntity<string>
{
    public string Key { get; set; } = default!;
    public BsonDocument Value { get; set; } = default!;
}
