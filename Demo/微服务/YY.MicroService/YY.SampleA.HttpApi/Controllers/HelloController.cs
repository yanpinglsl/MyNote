using Microsoft.AspNetCore.Mvc;

namespace YY.Sample.HttpApi.Controllers
{
    [Route("api/Test/[controller]")]
    [ApiController]
    public class HelloController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hello Test");
        }
    }
}
