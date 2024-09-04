using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Xml.Linq;

namespace YY.ConfigAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController(IConfiguration configuration) : ControllerBase
    {
        [HttpGet("name")]
        public IActionResult Get(string name)
        {
            return Ok($"当前配置{name}的值是：{configuration.GetSection(name)?.Value}");
        }
    }
}
