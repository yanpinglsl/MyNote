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
//using YY.Zhihu.UseCases.AppUsers.Commands;
//using YY.Zhihu.UseCases.Interfaces;

//namespace YY.Zhihu.UseCases.AppUsers.Queries
//{
//    public record LoginUserQuery(string Username, string Password) : IQuery<Result<TokenDto>>;
//    public class LoginUser(
//        IIdentityService identityService) : IQueryHandler<LoginUserQuery, Result<TokenDto>>
//    {
//        public async Task<Result<TokenDto>> Handle(LoginUserQuery request, CancellationToken cancellationToken)
//        {
//            var result = await identityService.GetAccessTokenAsync(request.Username, request.Password);
//            if (result.IsSuccess)
//            {
//                var token = new TokenDto(result.Value!);
//                return Result.Success(token);
//            }
//            return Result.From(result);
//        }
//    }
//}
