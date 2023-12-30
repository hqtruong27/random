using Microsoft.AspNetCore.Mvc;

namespace GenshinImpact.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class GachaHistoryController(IDispatcher dispatcher) : ControllerBase
{
    [HttpPost(Name = "create")]
    public async Task<IActionResult> Create([FromBody] CrawlGachaHistoryCommand command)
    {
        return Ok(await dispatcher.Send(command));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] long id)
    {
        return Ok(await dispatcher.Send(new GetGachaHistoryQuery(id)));
    }

    [HttpGet("WishCalculator")]
    public async Task<IActionResult> WishCalculatorAsync()
    {
        return Ok(await dispatcher.Send(new WishCalculatorQuery()));
    }
}