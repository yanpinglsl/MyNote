using YY.Zhihu.Domain.AppUserAggerate.Entites;
using YY.Zhihu.Domain.AppUserAggerate.Specifications;
using YY.Zhihu.Domain.Interfaces;
using YY.Zhihu.Domain.QuestionAggerate.Entites;
using YY.Zhihu.SharedLibraries.Message;
using YY.Zhihu.SharedLibraries.Repositoy;
using YY.Zhihu.SharedLibraries.Result;
using YY.Zhihu.UseCases.Common.Interfaces;

namespace YY.Zhihu.UseCases.AppUsers.Commands
{
    public record CreateFollowQuestionCommand(int QuestionId) : ICommand<IResult>;
    public class CreateFollowQuestion(
        IRepository<AppUser> userRepo,
        IAppUserService appUserService,
        IUser user) : ICommandHandler<CreateFollowQuestionCommand, IResult>
    {
        public async Task<IResult> Handle(CreateFollowQuestionCommand request, CancellationToken cancellationToken)
        {
            //var user = await userRepo.GetByIdAsync(request.userId);
            //if (user == null)
            //    return Result.NotFound("用户不存在");

            var spec = new FollowQuestionByIdSpec(user.Id!.Value, request.QuestionId);
            var appuser = await userRepo.GetSingleOrDefaultAsync(spec, cancellationToken);
            if (appuser == null) return Result.NotFound("用户不存在");
            var result = await appUserService.FollowQuestionAsync(appuser, request.QuestionId, cancellationToken);
            if (result.IsSuccess)
            {
                await userRepo.SaveChangesAsync(cancellationToken);
                return Result.Success(result);
            }
            else
            {
                return result;
            }
        }
    }
}
