﻿using AutoMapper;
using YY.Zhihu.Domain.AppUserAggerate.Entites;
using YY.Zhihu.SharedLibraries.Message;
using YY.Zhihu.SharedLibraries.Repositoy;
using YY.Zhihu.SharedLibraries.Result;
using YY.Zhihu.UseCases.Common.Attributes;

namespace YY.Zhihu.UseCases.AppUsers.Commands
{
    [Authorize]
    public record CreateAppUserCommand(int UserID) : ICommand<Result<AppUserDto>>;
    public class CreateAppUserHandler(
        IRepository<AppUser> userRepo,
        IMapper mapper) : ICommandHandler<CreateAppUserCommand, Result<AppUserDto>>
    {
        public async Task<Result<AppUserDto>> Handle(CreateAppUserCommand command, CancellationToken cancellationToken)
        {
            var user = userRepo.Add(new AppUser(command.UserID)
            {
                Nickname = $"新用户-{command.UserID}"
            });
            await userRepo.SaveChangesAsync();
            return Result.Success(mapper.Map<AppUserDto>(user));
        }
    }
}
