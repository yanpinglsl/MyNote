//using AutoMapper;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using YY.Zhihu.Domain.AppUserAggerate.Entites;
//using YY.Zhihu.Domain.Data;
//using YY.Zhihu.SharedLibraries.Message;
//using YY.Zhihu.SharedLibraries.Result;
//using YY.Zhihu.UseCases.Contracts.Interfaces;
//using YY.Zhihu.UseCases.Interfaces;

//namespace YY.Zhihu.UseCases.AppUsers.Commands
//{
//    public record RegisterUserCommand(string Username, string Password) : ICommand<Result<AppUserDto>>;
//    public class RegisterUserHandler(
//        IIdentityService identityService,
//        IRepository<AppUser> userRepo,
//        IMapper mapper) : ICommandHandler<RegisterUserCommand, Result<AppUserDto>>
//    {
//        public async Task<Result<AppUserDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
//        {
//            var result = await identityService.CreateUserAsync(request.Username, request.Password);
//            if (result.IsSuccess)
//            {
//                var user = userRepo.Add(new AppUser(result.Value)
//                {
//                    Nickname = $"新用户-{result.Value}"
//                });
//                await userRepo.SaveChangesAsync();
//                return Result.Success(mapper.Map<AppUserDto>(user));
//            }
//            return Result.Failure(result.Errors);
//        }
//    }
//}
