using MediatR;
using Microsoft.AspNetCore.Mvc;
using YY.Zhihu.Application.Infrastructure;
using YY.Zhihu.Application.Models;
using YY.Zhihu.Domain.QuestionAggerate.Entites;
using YY.Zhihu.SharedLibraries.Paging;
using YY.Zhihu.UseCases.Answers.Commands;
using YY.Zhihu.UseCases.Answers.Queries;
using YY.Zhihu.UseCases.AppUsers.Queries;
using YY.Zhihu.UseCases.Questions.Queries;

namespace YY.Zhihu.Application.Controllers
{
    [Route("api/question/{questionid:int}/answer")]
    public class AnswerController() : ApiControllerBase
    {
        [HttpGet("{answerid:int}")]
        public async Task<IActionResult> Get(int questionid, int answerid)
        {
            var result = await Sender.Send(new GetAnswerWithQuestion(questionid, answerid));
            return ReturnResult(result);
        }
        [HttpGet("list")]
        public async Task<IActionResult> GetList(int questionid, [FromQuery] Pagination pagination)
        {
            var result = await Sender.Send(new GetAnswerList(questionid, pagination));
            return ReturnResult(result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateAnswer(int questionid, CreateAnswerRequest request)
        {
            var result = await Sender.Send(new CreateAnswerCommand(questionid, request.Content));
            return ReturnResult(result);
        }
        [HttpPut("{answerid:int}")]
        public async Task<IActionResult> UpdateAnswer(int questionid, int answerid, UpdateAnswerRequest request)
        {
            var result = await Sender.Send(new UpdateAnswerCommand(questionid, answerid, request.Content));
            return ReturnResult(result);
        }
        [HttpDelete("{answerid:int}")]
        public async Task<IActionResult> DeleteAnswer(int questionid, int answerid)
        {
            var result = await Sender.Send(new DeleteAnswerCommand(questionid, answerid));
            return ReturnResult(result);
        }
        [HttpPost("{answerid:int}/like")]
        public async Task<IActionResult> CreateAnswerLike(int answerid,bool isLike)
        {
            var result = await Sender.Send(new CreateAnswerLikeCommand(answerid, isLike));
            return ReturnResult(result);
        }
        [HttpPut("{answerid:int}/like")]
        public async Task<IActionResult> UpdateAnswerLike(int answerid, bool isLike)
        {
            var result = await Sender.Send(new UpdateAnswerLikeCommand(answerid, isLike));
            return ReturnResult(result);
        }
        [HttpDelete("{answerid:int}/like")]
        public async Task<IActionResult> DeleteAnswerLike(int answerid)
        {
            var result = await Sender.Send(new DeleteAnswerLikeCommand(answerid));
            return ReturnResult(result);
        }
    }
}
