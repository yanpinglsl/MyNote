using Microsoft.AspNetCore.Mvc;
using YY.MyAService.HttpApi.Clients;

namespace YY.MyAService.HttpApi.Controllers
{
    /// <summary>
    /// ����ASP.NET���ɸ��ؾ���
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
