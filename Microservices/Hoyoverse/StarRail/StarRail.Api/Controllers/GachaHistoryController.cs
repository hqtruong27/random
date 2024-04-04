using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace StarRail.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class GachaHistoryController(IMediator mediator) : ControllerBase
{
    [HttpPost("crawl", Name = "crawl", Order = 1)]
    public async Task<IActionResult> Get([FromBody] string url)
    {
        return Ok(await mediator.Send(new CrawlGachaHistoryCommand(url)));
    }

    [HttpGet("WishCalculator")]
    public async Task<IActionResult> WishCalculatorAsync()
    {
        return Ok(await mediator.Send(new WishCalculatorQuery()));
    }
}
