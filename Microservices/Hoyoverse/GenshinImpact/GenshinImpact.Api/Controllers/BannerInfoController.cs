using Microsoft.AspNetCore.Mvc;

namespace GenshinImpact.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannerInfoController(IBannerInfoService bannerInfoService) : ControllerBase
    {
        private readonly IBannerInfoService _bannerInfoService = bannerInfoService;

        [HttpPost]
        public async Task<IActionResult> Create(BannerInfoRequest request)
        {
            await _bannerInfoService.CreateAsync(request);
            return Ok();
        }
    }
}
