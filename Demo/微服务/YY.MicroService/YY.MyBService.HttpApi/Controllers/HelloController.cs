using Microsoft.AspNetCore.Mvc;

namespace YY.MyBService.HttpApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelloController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hello");
        }
    }
}
