using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.SharedLibraries.Message;
using YY.Zhihu.SharedLibraries.Paging;
using YY.Zhihu.SharedLibraries.Result;
using YY.Zhihu.UseCases.Answers.Dto;
using YY.Zhihu.UseCases.Common.Attributes;
using YY.Zhihu.UseCases.Interfaces;
using YY.Zhihu.UseCases.Questions.Dto;

namespace YY.Zhihu.UseCases.Answers.Queries
{
    [Authorize]
    public record GetAnswerList(int QuestionId, Pagination Pagination) : IQuery<Result<PagedList<AnswerDto>>>;

    public class GetAnswerListHandler
        (IDataQueryService queryService) : IQueryHandler<GetAnswerList, Result<PagedList<AnswerDto>>>
    {
        public async Task<Result<PagedList<AnswerDto>>> Handle(GetAnswerList request, CancellationToken cancellationToken)
        {

            var answers = queryService.Answers;
            var appUsers = queryService.AppUsers;
            var questions = queryService.Questions;
            var queryable = from answer in answers
                            join user in appUsers
                            on answer.CreatedBy equals user.Id
                            where answer.QuestionId == request.QuestionId
                            orderby answer.LikeCount descending
                            select new AnswerDto
                            {
                                Id = answer.Id,
                                Content = answer.Content,
                                LikeCount = answer.LikeCount,
                                LastModifiedAt = answer.LastModifiedAt,
                                CreatedBy = answer.CreatedBy,
                                CreatedByNickName = user.Nickname,
                                CreatedByBio = user.Bio
                            };
            var result = await queryService.ToPageListAsync(queryable, request.Pagination);
            return result.Count == 0 ? Result.NotFound("回答不存在") : Result.Success(result);
        }
    }
}
