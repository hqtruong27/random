using Hoyolab.Api.Features.Activities;
using Microsoft.AspNetCore.Mvc;

namespace Hoyolab.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ActivityController(IDispatcher dispatcher) : ControllerBase
{
    [HttpPost("check-in")]
    public async Task<IActionResult> CheckIn([FromBody] CheckInCommand command)
    {
        return Ok(await dispatcher.Send(command));
    }
}