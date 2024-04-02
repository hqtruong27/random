using Microsoft.AspNetCore.Mvc;

namespace GenshinImpact.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class GachaHistoryController(IDispatcher dispatcher) : ControllerBase
{
    [HttpPost("crawl")]
    public async Task<IActionResult> Crawl([FromBody] CrawlGachaHistoryCommand command)
    {
        return Ok(await dispatcher.Send(command));
    }

    [HttpGet]
    public async Task<IActionResult> GetGachaHistories()
    {
        return Ok(await dispatcher.Send(new GetGachaHistoriesQuery()));
    }

    [HttpGet("WishCalculator")]
    public async Task<IActionResult> WishCalculatorAsync()
    {
        return Ok(await dispatcher.Send(new WishCalculatorQuery()));
    }
}