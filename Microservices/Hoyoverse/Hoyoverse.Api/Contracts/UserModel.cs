namespace Hoyoverse.Contracts;

public class UserModel
{
    public List<HoyolabModel> Hoyolabs { get; set; } = [];
}

public class HoyolabModel
{
    public required string Id { get; set; }
    public required string Cookie { get; set; }
    public string UserName { get; set; } = default!;
    public string Gender { get; set; } = default!;
    public bool IsAutoCheckIn { get; set; } = default!;
}