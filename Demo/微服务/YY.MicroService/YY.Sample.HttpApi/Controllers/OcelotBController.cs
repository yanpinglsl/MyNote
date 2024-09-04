using Microsoft.AspNetCore.Mvc;

namespace YY.Sample.HttpApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OcelotBController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hello,Ocelot");
        }
        [HttpGet("test/say")]
        public IActionResult SayHello()
        {
            return Ok("Say Hello");
        }
        [HttpGet("say")]
        public IActionResult GetName()
        {
            return Ok("Jenny");
        }
    }
}
