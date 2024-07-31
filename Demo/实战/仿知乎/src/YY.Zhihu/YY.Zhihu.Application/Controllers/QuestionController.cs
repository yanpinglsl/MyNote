using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YY.Zhihu.Application.Infrastructure;
using YY.Zhihu.Application.Models;
using YY.Zhihu.UseCases.AppUsers.Commands;
using YY.Zhihu.UseCases.Questions.Queries;

namespace YY.Zhihu.Application.Controllers
{
    [Route("api/question")]
    [ApiController]
    public class QuestionController : ApiControllerBase
    {
        [HttpGet("{questionid:int}")]
        public async Task<IActionResult> Get(int questionid)
        {
            var result = await Sender.Send(new GetQuestionQuery(questionid));
            return ReturnResult(result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateQuestion(CreateQuestionRequest request)
        {
            var result = await Sender.Send(new CreateQuestionCommand(request.Title, request.Description));
            return ReturnResult(result);
        }
        [HttpPut("{questionid:int}")]
        public async Task<IActionResult> UpdateQuestion(int questionid, UpdateQuestionRequest request)
        {
            var result = await Sender.Send(new UpdateQuestionCommand(questionid, request.Title, request.Description));
            return ReturnResult(result);
        }
        [HttpDelete("{questionid:int}")]
        public async Task<IActionResult> DeleteQuestion(int questionid)
        {
            var result = await Sender.Send(new DeleteQuestionCommand(questionid));
            return ReturnResult(result);
        }
    }
}
