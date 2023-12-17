namespace GenshinImpact.Services.Interfaces;

public interface IBannerInfoService
{
    Task CreateAsync(BannerInfoRequest request);
}
