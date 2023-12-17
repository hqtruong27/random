using Microsoft.AspNetCore.Mvc;

namespace GenshinImpact.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannerInfoController(BannerInfoService bannerInfoService) : ControllerBase
    {
        private readonly IBannerInfoService _bannerInfoService = bannerInfoService;

        [HttpPost]
        public async Task<IActionResult> Create(IBannerInfoService request)
        {
            await _bannerInfoService.CreateAsync(request);
            return Ok();
        }
    }
}
