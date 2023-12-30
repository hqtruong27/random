namespace GenshinImpact.Services.Interfaces;

public interface IGachaHistoryService
{
    IAsyncEnumerable<WishCounterModel> WishCalculatorAsync();
}
