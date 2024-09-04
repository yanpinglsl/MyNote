using Microsoft.AspNetCore.Mvc;

namespace YY.SampleA.HttpApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OcelotController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hello,Ocelot");
        }
        [HttpGet("say")]
        public IActionResult GetName()
        {
            return Ok("Jenny");
        }
    }
}
