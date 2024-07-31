﻿using YY.Zhihu.Domain.AppUserAggerate.Entites;
using YY.Zhihu.Domain.AppUserAggerate.Specifications;
using YY.Zhihu.Domain.Interfaces;
using YY.Zhihu.SharedLibraries.Message;
using YY.Zhihu.SharedLibraries.Repositoy;
using YY.Zhihu.SharedLibraries.Result;
using YY.Zhihu.UseCases.Common.Attributes;
using YY.Zhihu.UseCases.Contracts.Interfaces;

namespace YY.Zhihu.UseCases.AppUsers.Commands
{
    [Authorize]
    public record DeleteFolloweeUserCommand(int FolloweeId) : ICommand<IResult>;
    public class DeleteFolloweeUser(
        IRepository<AppUser> userRepo,
        IAppUserService appUserService,
        IUser user) : ICommandHandler<DeleteFolloweeUserCommand, IResult>
    {
        public async Task<IResult> Handle(DeleteFolloweeUserCommand request, CancellationToken cancellationToken)
        {
            var spec = new FolloweeUserByIdSpec(user.Id!.Value, request.FolloweeId);
            var appuser = await userRepo.GetSingleOrDefaultAsync(spec, cancellationToken);
            if (appuser == null)
                return Result.NotFound("用户不存在");
            appuser.RemoveFolloweeUser(request.FolloweeId);
            await userRepo.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
