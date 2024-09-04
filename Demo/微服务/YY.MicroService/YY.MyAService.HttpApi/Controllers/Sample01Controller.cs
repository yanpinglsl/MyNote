using Microsoft.AspNetCore.Mvc;
using YY.Common.Consul.ServiceDiscovery;

namespace YY.MyAService.HttpApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Sample01Controller(IServiceClient serviceClient) : ControllerBase
    {
        [HttpGet("name")]
        public async Task<IActionResult> Get(string name)
        {
            var services = await serviceClient.GetServicesAsync(name);
            return Ok(services);
        }
    }
}
