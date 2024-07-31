using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.SharedLibraries.Result;
using YY.Zhihu.UseCases.Contracts.Interfaces;

namespace YY.Zhihu.HotService.Data
{
    public class QuestionStatQuery(IDataQueryService dataQueryService)
    {
        public async Task<Result<Dictionary<int,QuestionStat>>> GetLatest()
        {
            //var createdAtBegin = DateTimeOffset.Now.AddDays(-7);
            //var lastModifiedBegin = DateTimeOffset.Now.AddHours(-48);

            var questions = dataQueryService.Questions;
            var answers = dataQueryService.Answers;
            var queryable = from question in questions
                         join answer in answers
                         on question.Id equals answer.QuestionId
                         group answer by new
                         {
                             question.Id,
                             question.ViewCount,
                             question.FollowerCount,
                             question.LastModifiedAt,
                             question.CreatedAt
                         } into g
                         //where g.Key.CreatedAt >= createdAtBegin
                         //&& g.Key.LastModifiedAt >= lastModifiedBegin
                         orderby g.Key.ViewCount descending
                         select new
                         {
                             Id = g.Key.Id,
                             ViewCount = g.Key.ViewCount,
                             FollowCount = g.Key.FollowerCount,
                             AnswerCount = g.Count(),
                             LikeCount = g.Sum(answer => answer.LikeCount)
                         };
            var questionList = await dataQueryService.ToListAsync(queryable);
            if (questionList.Count == 0) return Result.NotFound();

            var questionStats = questionList.ToDictionary(item => item.Id, item => new QuestionStat
            {
                ViewCount = item.ViewCount,
                FollowCount = item.FollowCount,
                AnswerCount = item.AnswerCount,
                LikeCount = item.LikeCount
            });

            return Result.Success(questionStats);
        }
    }
}
