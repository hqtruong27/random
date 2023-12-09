using Microsoft.AspNetCore.Mvc;
using Models.GenshinImpact;
using System.Text.Json;

namespace GenshinImpact.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GachaHistoryController : ControllerBase
    {
        private readonly ILogger<GachaHistoryController> _logger;

        public GachaHistoryController(ILogger<GachaHistoryController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "create")]
        public async Task<IActionResult> Create(string url)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(url);

            var data = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<GachaInfoResponse>(data);
            return Ok(data);
        }
    }
}
