using Microsoft.AspNetCore.Mvc;

namespace GenshinImpact.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannerInfoController(IDispatcher dispatcher) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create(CreateBannerCommand command)
        {
            await dispatcher.Send(command);
            return Ok();
        }
    }
}
