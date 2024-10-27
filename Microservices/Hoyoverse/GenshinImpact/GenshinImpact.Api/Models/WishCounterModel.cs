using GenshinImpact.Shared.EnumTypes;

namespace GenshinImpact.Api.Models;

public class WishCounterModel
{
    public BannerType Banner { get; set; }
    public WishBanner Detail { get; set; } = new();
}

public class WishBanner
{
    public long TotalPulls { get; set; }
    public int Pity { get; set; }
    public IEnumerable<Event> Events { get; set; } = [];
}

public class Event
{
    public string ItemName { get; set; } = string.Empty;
    public int PullIndex { get; set; }
}