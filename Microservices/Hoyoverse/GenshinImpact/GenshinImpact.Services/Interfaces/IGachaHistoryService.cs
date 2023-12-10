namespace GenshinImpact.Services.Interfaces;

public interface IGachaHistoryService
{
    IAsyncEnumerable<long> CrawlAsync(string authKey);
    Task<GachaHistoryResponse> FindByIdAsync(long id);
}
