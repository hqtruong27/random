namespace Models.Hoyolab;

public record ActivityConfig
{
    public string CheckInUrl { get; set; } = default!;
    public Act Act { get; set; } = new();

}

public record Act
{
    public string Genshin { get; set; } = default!;
    public string Hsr { get; set; } = default!;
    public string Hi3 { get; set; } = default!;
}