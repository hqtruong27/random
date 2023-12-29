using Core;
using Hoyolab.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Hoyolab;

namespace Hoyolab.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ActivityController(IActivityService activityService, IDispatcher dispatcher) : ControllerBase
    {
        [HttpPost("check-in")]
        public async Task<IActionResult> CheckIn([FromBody] CheckInRequest request)
        {
            return Ok(await activityService.CheckInAsync(request));
        }

        [HttpPost("test")]
        public async Task<IActionResult> Test([FromBody] CheckInRequest request)
        {
            return Ok(await dispatcher.Send(request));
        }
    }
}
