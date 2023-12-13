namespace GenshinImpact.Services.Interfaces;

public interface IGachaHistoryService
{
    IAsyncEnumerable<long> CrawlAsync(string url);
    Task<GachaHistoryResponse> FindByIdAsync(long id);
    IAsyncEnumerable<WishCounterModel> WishCalculatorAsync();
}
