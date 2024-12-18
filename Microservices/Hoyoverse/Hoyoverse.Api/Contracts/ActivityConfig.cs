namespace Hoyoverse.Contracts;

public record ActivityConfig
{
    public Config Genshin { get; set; } = new();
    public Config Hsr { get; set; } = new();
    public Config Hi3 { get; set; } = new();
}

public record Config
{
    public string ActId { get; set; } = default!;
    public string CheckInUrl { get; set; } = default!;
}