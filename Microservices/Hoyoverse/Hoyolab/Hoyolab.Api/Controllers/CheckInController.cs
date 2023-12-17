using Hoyolab.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Hoyolab.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CheckInController(ILogger<CheckInController> logger, ICheckInService checkInService) : ControllerBase
    {
        private readonly ILogger<CheckInController> _logger = logger;
        private readonly ICheckInService _checkInService = checkInService;

        [HttpPost]
        public async Task<IActionResult> CheckIn()
        {
            return Ok(await _checkInService.CheckInAsync(new()));
        }
    }
}
