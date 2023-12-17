﻿using MongoDB.Bson;

namespace Hoyoverse.Infrastructure.Entities;

public class User : AuditableEntity<ObjectId>
{
    public Discord Discord { get; set; } = new();
    public Hoyolab Hoyolab { get; set; } = new();
}


public class Hoyolab
{
    public string Id { get; set; } = default!;
    public string Cookie { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public bool IsAutoCheckIn { get; set; } = default!;
}

public class Discord
{
    public string Id { get; set; } = default!;
    public string UserName { get; set; } = default!;
}