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
    public record CreateFolloweeUserCommand(int FolloweeId) : ICommand<IResult>;
    public class CreateFolloweeUser(
        IRepository<AppUser> userRepo,
        IAppUserService appUserService,
        IUser user) : ICommandHandler<CreateFolloweeUserCommand, IResult>
    {
        public async Task<IResult> Handle(CreateFolloweeUserCommand request, CancellationToken cancellationToken)
        {
            var spec = new FolloweeUserByIdSpec(user.Id!.Value, request.FolloweeId);
            var appuser = await userRepo.GetSingleOrDefaultAsync(spec, cancellationToken);
            if (appuser == null)
                return Result.NotFound("用户不存在");
            var result = await appUserService.FolloweeUserAsync(appuser, request.FolloweeId, cancellationToken);
            if (!result.IsSuccess)
                return result;
            await userRepo.SaveChangesAsync(cancellationToken);
            return Result.Success(result);
        }
    }
}
