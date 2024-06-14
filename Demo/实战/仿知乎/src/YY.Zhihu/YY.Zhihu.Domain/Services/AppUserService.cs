using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.Domain.AppUserAggerate.Entites;
using YY.Zhihu.Domain.AppUserAggerate.Specifications;
using YY.Zhihu.Domain.Interfaces;
using YY.Zhihu.Domain.QuestionAggerate.Entites;
using YY.Zhihu.SharedLibraries.Repositoy;
using YY.Zhihu.SharedLibraries.Result;

namespace YY.Zhihu.Domain.Services
{
    public class AppUserService(
        IRepository<Question> questions,
        IRepository<AppUser> appUsers) : IAppUserService
    {
        public async Task<IResult> FollowQuestionAsync(AppUser appuser, int questionId, CancellationToken cancellationToken)
        {
            //var question = await questions.GetByIdAsync(questionId);
            // if(question == null)
            // {
            //     return Result.NotFound("关注问题不存在");
            // }
            // question.FollowerCount += 1;

            var question = await questions.GetByIdAsync(questionId, cancellationToken);
            if (question == null) 
                return Result.NotFound("关注问题不存在");
            var result = appuser.AddFollowQuestion(questionId);
            return Result.From(result);
        }

        public async Task<IResult> FolloweeUserAsync(AppUser appuser, int followeeId, CancellationToken cancellationToken)
        {
            if (await appUsers.CountAsync(new AppUserByIdSpec(followeeId), cancellationToken) == 0)
            {
                return Result.NotFound("关注用户不存在");
            }

            return appuser.AddFolloweeUser(followeeId);
        }
    }
}
