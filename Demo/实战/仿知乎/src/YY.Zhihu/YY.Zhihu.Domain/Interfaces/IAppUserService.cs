
using YY.Zhihu.Domain.AppUserAggerate.Entites;
using YY.Zhihu.SharedLibraries.Result;

namespace YY.Zhihu.Domain.Interfaces;

public interface IAppUserService
{
    Task<IResult> FollowQuestionAsync(AppUser appuser, int questionId, CancellationToken cancellationToken);
    Task<IResult> FolloweeUserAsync(AppUser appuser, int followeeId, CancellationToken cancellationToken);
}
