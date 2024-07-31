using Microsoft.AspNetCore.Mvc;
using YY.Zhihu.Application.Infrastructure;
using YY.Zhihu.UseCases.AppUsers.Commands;
using YY.Zhihu.UseCases.AppUsers.Queries;

namespace YY.Zhihu.Application.Controllers
{
    [Route("api/appuser")]
    public class AppUserController() : ApiControllerBase
    {
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int userid)
        {
            var result = await Sender.Send(new GetUserInfoQuery(userid));

            return ReturnResult(result);
        }

        [HttpPut("follow/question/{questionid:int}")]
        public async Task<IActionResult> CreateFollowQuestion(int questionid)
        {
            var result = await Sender.Send(new CreateFollowQuestionCommand(questionid));

            return ReturnResult(result);
        }

        [HttpDelete("follow/question/{questionid:int}")]
        public async Task<IActionResult> DeleteFollowQuestion(int questionid)
        {
            var result = await Sender.Send(new DeleteFollowQuestionCommand(questionid));
            return ReturnResult(result);
        }

        [HttpPut("follow/user/{userid:int}")]
        public async Task<IActionResult> CreateFolloweeUser(int userid)
        {
            var result = await Sender.Send(new CreateFolloweeUserCommand(userid));
            return ReturnResult(result);
        }

        [HttpDelete("follow/user/{userid:int}")]
        public async Task<IActionResult> DeleteFolloweeUser(int userid)
        {
            var result = await Sender.Send(new DeleteFolloweeUserCommand(userid));
            return ReturnResult(result);
        }
    }
}
