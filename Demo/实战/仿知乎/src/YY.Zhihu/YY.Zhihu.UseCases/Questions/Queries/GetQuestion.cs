using YY.Zhihu.SharedLibraries.Message;
using YY.Zhihu.SharedLibraries.Result;
using YY.Zhihu.UseCases.Common.Attributes;
using YY.Zhihu.UseCases.Interfaces;
using YY.Zhihu.UseCases.Questions.Dto;

namespace YY.Zhihu.UseCases.Questions.Queries
{
    [Authorize]
    public record GetQuestionQuery(int Id) : IQuery<Result<QuestionDto>>;

    public class GetQuestionQueryHandler
        (IDataQueryService queryService) : IQueryHandler<GetQuestionQuery, Result<QuestionDto>>
    {
        public async Task<Result<QuestionDto>> Handle(GetQuestionQuery request, CancellationToken cancellationToken)
        {
            var queryable = queryService.Questions
                .Where(u => u.Id == request.Id)
                .Select(u => new QuestionDto()
                {
                    Id = u.Id,
                    Title = u.Title,
                    Description = u.Description,
                    AnswerCount = u.Answers.Count,
                    FollowerCount = u.FollowerCount,
                    ViewCount = u.ViewCount

                });

            var question = await queryService.FirstOrDefaultAsync(queryable);

            return question is null ? Result.NotFound("问题不存在") : Result.Success(question);
        }
    }
}
