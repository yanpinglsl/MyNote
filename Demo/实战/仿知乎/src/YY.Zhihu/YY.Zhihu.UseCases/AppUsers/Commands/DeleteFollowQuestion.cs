using YY.Zhihu.Domain.AppUserAggerate.Entites;
using YY.Zhihu.Domain.AppUserAggerate.Specifications;
using YY.Zhihu.Domain.Interfaces;
using YY.Zhihu.SharedLibraries.Message;
using YY.Zhihu.SharedLibraries.Repositoy;
using YY.Zhihu.SharedLibraries.Result;
using YY.Zhihu.UseCases.Common.Interfaces;

namespace YY.Zhihu.UseCases.AppUsers.Commands
{
    public record DeleteFollowQuestionCommand(int QuestionId) : ICommand<IResult>;
    public class DeleteFollowQuestion(
        IRepository<AppUser> userRepo,
        IAppUserService appUserService,
        IUser user) : ICommandHandler<DeleteFollowQuestionCommand, IResult>
    {
        public async Task<IResult> Handle(DeleteFollowQuestionCommand request, CancellationToken cancellationToken)
        {
            //var user = await userRepo.GetByIdAsync(request.userId);
            //if (user == null)
            //    return Result.NotFound("用户不存在");
            FollowQuestionByIdSpec spec = new FollowQuestionByIdSpec(user.Id!.Value, request.QuestionId);
            var appuser =await userRepo.GetSingleOrDefaultAsync(spec, cancellationToken);
            if(appuser == null)
                return Result.NotFound("用户不存在");
            appuser.RemoveFollowQuestion(request.QuestionId);
            await userRepo.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
