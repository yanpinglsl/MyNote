using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YY.Zhihu.Application.Infrastructure;
using YY.Zhihu.Application.Models;
using YY.Zhihu.Infrastructure.Identity;
using YY.Zhihu.UseCases.AppUsers.Commands;

namespace YY.Zhihu.Application.Controllers
{
    public class IdentityController (IdentityService identityService) : ApiControllerBase
    {
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser(UserRegisterRequest request)
        {
            var identityResult = await identityService.CreateUserAsync(request.Username, request.Password);

            if (!identityResult.IsSuccess) return ReturnResult(identityResult);

            var result = await Sender.Send(new CreateAppUserCommand(identityResult.Value));

            return ReturnResult(result);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginRequest request)
        {
            var result = await identityService.GetAccessTokenAsync(request.Username, request.Password);

            if (!result.IsSuccess) 
                return ReturnResult(result);

            return Ok(new
            {
                AccessToken = result.Value
            });
        }

        [HttpPost("Test")]
        public async Task<IActionResult> Test()
        {
            return Ok(new
            {
                UserName = User.FindFirstValue(ClaimTypes.Name)
            }); 
        }
    }
}
