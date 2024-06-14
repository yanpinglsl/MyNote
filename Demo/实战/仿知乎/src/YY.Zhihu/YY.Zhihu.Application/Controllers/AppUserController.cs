using Microsoft.AspNetCore.Mvc;
using YY.Zhihu.Application.Infrastructure;
using YY.Zhihu.UseCases.AppUsers.Queries;

namespace YY.Zhihu.Application.Controllers
{
    public class AppUserController() : ApiControllerBase
    {
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await Sender.Send(new GetUserInfoQuery(id));

            return ReturnResult(result);
        }
    }
}
