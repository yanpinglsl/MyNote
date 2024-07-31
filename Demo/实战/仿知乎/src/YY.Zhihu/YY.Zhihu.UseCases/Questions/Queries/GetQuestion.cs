using MediatR;
using YY.Zhihu.Domain.QuestionAggerate.Event;
using YY.Zhihu.SharedLibraries.Message;
using YY.Zhihu.SharedLibraries.Result;
using YY.Zhihu.UseCases.Common.Attributes;
using YY.Zhihu.UseCases.Contracts.Common.Interfaces;
using YY.Zhihu.UseCases.Contracts.Interfaces;
using YY.Zhihu.UseCases.Questions.Dto;
using YY.Zhihu.UseCases.Questions.Jobs;

namespace YY.Zhihu.UseCases.Questions.Queries
{
    [Authorize]
    public record GetQuestionQuery(int Id) : IQuery<Result<QuestionDto>>;

    public class GetQuestionQueryHandler
        (IDataQueryService queryService,
         ICacheService<QuestionDto> cacheService,
        //QuestionViewCountService questionViewCount,
        IPublisher publisher) : IQueryHandler<GetQuestionQuery, Result<QuestionDto>>
    {
        public async Task<Result<QuestionDto>> Handle(GetQuestionQuery request, CancellationToken cancellationToken)
        {
            //var queryable = queryService.Questions
            //    .Where(u => u.Id == request.Id)
            //    .Select(u => new QuestionDto()
            //    {
            //        Id = u.Id,
            //        Title = u.Title,
            //        Description = u.Description,
            //        AnswerCount = u.Answers.Count,
            //        FollowerCount = u.FollowerCount,
            //        ViewCount = u.ViewCount

            //    });

            //var question = await queryService.FirstOrDefaultAsync(queryable);
            var question = await cacheService.GetOrSetByIdAsync(request.Id, async _ =>
            {
                var queryable = queryService.Questions
                    .Where(q => q.Id == request.Id)
                    .Select(q => new QuestionDto
                    {
                        Id = q.Id,
                        Title = q.Title,
                        Description = q.Description,
                        AnswerCount = q.Answers.Count,
                        FollowerCount = q.FollowerCount,
                        ViewCount = q.ViewCount
                    });
                return await queryService.FirstOrDefaultAsync(queryable);
            });

            if (question == null)
            {
                return Result.NotFound("问题不存在");
            }
            else
            {
                //questionViewCount.AddViewCount(question.Id);
                await publisher.Publish(new QuestionViewedEvent(question.Id), cancellationToken);
                return Result.Success(question);
            }
        }
    }
}
