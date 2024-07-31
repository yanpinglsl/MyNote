using YY.Zhihu.SharedLibraries.Message;
using YY.Zhihu.SharedLibraries.Result;
using YY.Zhihu.UseCases.Answers.Dto;
using YY.Zhihu.UseCases.Common.Attributes;
using YY.Zhihu.UseCases.Contracts.Common.Interfaces;
using YY.Zhihu.UseCases.Contracts.Interfaces;
using YY.Zhihu.UseCases.Questions.Dto;

namespace YY.Zhihu.UseCases.Questions.Queries
{
    [Authorize]
    public record GetAnswerWithQuestion(int QuestionId, int AnswerId) : IQuery<Result<AnswerWithQuestionDto>>;

    public class GetAnswerListHandler
        (IDataQueryService queryService,
        ICacheService<AnswerWithQuestionDto> cacheService)
        : IQueryHandler<GetAnswerWithQuestion, Result<AnswerWithQuestionDto>>
    {
        public async Task<Result<AnswerWithQuestionDto>> Handle(GetAnswerWithQuestion request, CancellationToken cancellationToken)
        {    
            //var answers = queryService.Answers;
            //var appUsers = queryService.AppUsers;
            //var questions = queryService.Questions;

            //var queryable = from answer in answers
            //                join question in questions 
            //                on answer.QuestionId equals question.Id
            //                join user in appUsers
            //                on answer.CreatedBy equals user.Id
            //                where answer.Id == request.AnswerId && answer.QuestionId == request.QuestionId
            //                select new AnswerWithQuestionDto
            //                {
            //                    Answer = new AnswerDto
            //                    {
            //                        Id = answer.Id,
            //                        Content = answer.Content,
            //                        LikeCount = answer.LikeCount,
            //                        LastModifiedAt = answer.LastModifiedAt,
            //                        CreatedBy = answer.CreatedBy,
            //                        CreatedByNickName = user.Nickname,
            //                        CreatedByBio = user.Bio
            //                    },
            //                    Question = new QuestionDto
            //                    {
            //                        Id = question.Id,
            //                        Title = question.Title,
            //                        Description = question.Description,
            //                        AnswerCount = question.Answers.Count,
            //                        FollowerCount = question.FollowerCount,
            //                        ViewCount = question.ViewCount
            //                    }
            //                };
            //var result = await queryService.FirstOrDefaultAsync(queryable);

            var result = await cacheService.GetOrSetByIdAsync(request.QuestionId, request.AnswerId, async _ =>
            {
                var answers = queryService.Answers;
                var appUsers = queryService.AppUsers;
                var questions = queryService.Questions;

                var queryable = from answer in answers
                                join question in questions
                                on answer.QuestionId equals question.Id
                                join user in appUsers
                                on answer.CreatedBy equals user.Id
                                where answer.Id == request.AnswerId && answer.QuestionId == request.QuestionId
                                select new AnswerWithQuestionDto
                                {
                                    Answer = new AnswerDto
                                    {
                                        Id = answer.Id,
                                        Content = answer.Content,
                                        LikeCount = answer.LikeCount,
                                        LastModifiedAt = answer.LastModifiedAt,
                                        CreatedBy = answer.CreatedBy,
                                        CreatedByNickName = user.Nickname,
                                        CreatedByBio = user.Bio
                                    },
                                    Question = new QuestionDto
                                    {
                                        Id = question.Id,
                                        Title = question.Title,
                                        Description = question.Description,
                                        AnswerCount = question.Answers.Count,
                                        FollowerCount = question.FollowerCount,
                                        ViewCount = question.ViewCount
                                    }
                                };
                return await queryService.FirstOrDefaultAsync(queryable); 
            });
            return result == null ? Result.NotFound("回答不存在") : Result.Success(result);
        }
    }
}
