using Common.Enum.Hoyoverse;

namespace Models.GenshinImpact;

public class GachaHistoryConfig
{
    public GachaType Type { get; set; }
    public string Url { get; set; } = default!;
}
