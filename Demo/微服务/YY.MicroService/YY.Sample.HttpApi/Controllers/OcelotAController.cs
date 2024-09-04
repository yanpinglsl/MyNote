using Microsoft.AspNetCore.Mvc;

namespace YY.Sample.HttpApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OcelotAController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hello,Ocelot");
        }
        [HttpGet("test")]
        public IActionResult TestHello()
        {
            return Ok("Hello Test");
        }
        [HttpGet("say")]
        public IActionResult GetName()
        {
            return Ok("Jenny");
        }
        [HttpGet("cache")]
        public IActionResult GetCache()
        {
            Thread.Sleep(3000);
            return Ok("Cache");
        }
        [HttpGet("qosoptions")]
        public IActionResult GetQoSoption()
        {
            Thread.Sleep(3000);
            return Ok("QoSoption");
        }
        [HttpGet("exception")]
        public IActionResult GetException()
        {
            throw new Exception();
            return Ok("exception");
        }
    }
}
