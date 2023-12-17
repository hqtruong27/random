using Hoyolab.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Hoyolab;

namespace Hoyolab.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ActivityController(IActivityService activityService) : ControllerBase
    {
        private readonly IActivityService _activityService = activityService;

        [HttpPost("check-in")]
        public async Task<IActionResult> CheckIn([FromBody] CheckInRequest request)
        {
            return Ok(await _activityService.CheckInAsync(request));
        }
    }
}
