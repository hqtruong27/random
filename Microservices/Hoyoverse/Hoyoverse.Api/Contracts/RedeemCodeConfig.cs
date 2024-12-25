namespace Hoyoverse.Contracts;

[BsonIgnoreExtraElements]
public class RedeemCodeConfig
{
    public ProviderRedeemCodeConfig GenshinImpact { get; set; } = default!;
    public ProviderRedeemCodeConfig Hsr { get; set; } = default!;
}

public class ProviderRedeemCodeConfig
{
    public string UrlRedeem { get; set; } = default!;
    public string Url { get; set; } = default!;
}