using Microsoft.AspNetCore.Mvc;

namespace GenshinImpact.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class GachaHistoryController(IGachaHistoryService gachaHistoryService) : ControllerBase
{
    private readonly IGachaHistoryService _gachaHistoryService = gachaHistoryService;

    [HttpPost(Name = "create")]
    public async IAsyncEnumerable<long> Create([FromBody] string url)
    {
        await foreach (var item in _gachaHistoryService.CrawlAsync(url))
        {
            yield return item;
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] long id)
    {
        return Ok(await _gachaHistoryService.FindByIdAsync(id));
    }

    [HttpGet("WishCalculator")]
    public async IAsyncEnumerable<WishCounterModel> WishCalculatorAsync()
    {
        await foreach (var item in _gachaHistoryService.WishCalculatorAsync())
        {
            yield return item;
        }

    }
}