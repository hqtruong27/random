namespace Hoyoverse.Contracts;

public class WishListOptions
{
    public WishListUrl GenshinImpact { get; set; } = default!;
    public WishListUrl Hsr { get; set; } = default!;
}

public class WishListUrl
{
    public string GachaUrl { get; set; } = default!;
    public string GachaDetailUrl { get; set; } = default!;
}