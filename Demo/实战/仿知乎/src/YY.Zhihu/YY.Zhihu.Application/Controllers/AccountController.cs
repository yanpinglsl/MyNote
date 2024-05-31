//using Microsoft.AspNetCore.Mvc;
//using YY.Zhihu.Application.Infrastructure;
//using YY.Zhihu.UseCases.AppUsers.Commands;
//using YY.Zhihu.UseCases.AppUsers.Queries;

//namespace YY.Zhihu.Application.Controllers
//{
//    public class AccountController : ApiControllerBase
//    {
//        [HttpPost("Register")]
//        public async Task<IActionResult> RegisterUser(RegisterUserCommand command)
//        {
//          var result = await  Sender.Send(command);
//            return ReturnResult(result);
//        }

//        [HttpPost("Login")]
//        public async Task<IActionResult> Login(LoginUserQuery loginUser)
//        {
//            var result = await Sender.Send(loginUser);

//            return ReturnResult(result);
//        }
//    }
//}
