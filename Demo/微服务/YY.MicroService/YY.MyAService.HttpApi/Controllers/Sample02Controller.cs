using Microsoft.AspNetCore.Mvc;
using YY.MyAService.HttpApi.Clients;

namespace YY.MyAService.HttpApi.Controllers
{
    /// <summary>
    /// 测试ASP.NET集成负载均衡
    /// </summary>
    /// <param name="myBServiceClient"></param>
    [Route("api/[controller]")]
    [ApiController]
    public class Sample02Controller(MyBServiceClient myBServiceClient) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await myBServiceClient.GetHelloAsync();
            return Ok(result);
        }
    }
}
