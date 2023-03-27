using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebCoreExtend.ConfigurationExtend;

namespace WebCoreExtend.AuthenticationExtend
{
    /// <summary>
    /// 完全自定义的凭证格式和解析方式
    /// </summary>
    public class UrlTokenAuthenticationHandler : IAuthenticationHandler, IAuthenticationSignInHandler, IAuthenticationSignOutHandler
    {
        private AuthenticationScheme _AuthenticationScheme = null;
        private HttpContext _HttpContext = null;

        /// <summary>
        /// 初始化，Provider传递进来的
        /// 像方法注入
        /// </summary>
        /// <param name="scheme"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
        {
            Console.WriteLine($"This is {nameof(UrlTokenAuthenticationHandler)}.InitializeAsync");
            this._AuthenticationScheme = scheme;
            this._HttpContext = context;
            return Task.CompletedTask;
        }

        /// <summary>
        /// 核心鉴权处理方法,解析用户信息
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<AuthenticateResult> AuthenticateAsync()
        {
            Console.WriteLine($"This is {nameof(UrlTokenAuthenticationHandler)}.AuthenticateAsync");
            string userInfo = this._HttpContext.Request.Query["UrlToken"];//信息从哪里读
            Console.WriteLine($"获取token：{userInfo}");

            if (string.IsNullOrWhiteSpace(userInfo))
            {
                return Task.FromResult<AuthenticateResult>(AuthenticateResult.NoResult());//跳转登陆-ChallengeAsync
            }
            else if ("eleven-123456".Equals(userInfo))//信息是否可靠？  校验规则可以传递到Option的
            {
                var claimIdentity = new ClaimsIdentity("Custom");
                claimIdentity.AddClaim(new Claim(ClaimTypes.Name, userInfo));
                claimIdentity.AddClaim(new Claim(ClaimTypes.Role, "User"));
                claimIdentity.AddClaim(new Claim(ClaimTypes.Email, "xuyang@ZhaoxiEdu.Net"));
                claimIdentity.AddClaim(new Claim(ClaimTypes.Country, "China"));
                claimIdentity.AddClaim(new Claim(ClaimTypes.DateOfBirth, "1986"));
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimIdentity);//信息拼装和传递

                return Task.FromResult<AuthenticateResult>(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, null, _AuthenticationScheme.Name)));//往后传递？这里封装了的
            }
            else
            {
                return Task.FromResult<AuthenticateResult>(AuthenticateResult.Fail($"UrlToken is wrong: {userInfo}"));//ForbidAsync
            }
        }

        /// <summary>
        /// 未登录
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public Task ChallengeAsync(AuthenticationProperties properties)
        {
            Console.WriteLine($"This is {nameof(UrlTokenAuthenticationHandler)}.ChallengeAsync");
            string redirectUri = "/Home/Index";
            this._HttpContext.Response.Redirect(redirectUri);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 未授权，无权限
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public Task ForbidAsync(AuthenticationProperties properties)
        {
            Console.WriteLine($"This is {nameof(UrlTokenAuthenticationHandler)}.ForbidAsync");
            this._HttpContext.Response.StatusCode = 403;
            return Task.CompletedTask;
        }


        /// <summary>
        /// SignInAsync和SignOutAsync使用了独立的定义接口，
        /// 因为在现代架构中，通常会提供一个统一的认证中心，负责证书的颁发及销毁（登入和登出），
        /// 而其它服务只用来验证证书，并用不到SingIn/SingOut。
        /// </summary>
        /// <param name="user"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public Task SignInAsync(ClaimsPrincipal user, AuthenticationProperties properties)
        {
            var ticket = new AuthenticationTicket(user, properties, this._AuthenticationScheme.Name);
            this._HttpContext.Response.Cookies.Append("UrlTokenCookie", Newtonsoft.Json.JsonConvert.SerializeObject(ticket.Principal.Claims));
            //把一些信息再写入到前端cookie，客户端请求时，从coookie读取UrlTokenCookie信息，放到url上
            return Task.CompletedTask;
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public Task SignOutAsync(AuthenticationProperties properties)
        {
            this._HttpContext.Response.Cookies.Delete("UrlTokenCookie");
            return Task.CompletedTask;
        }

    }

    /// <summary>
    /// 提供个固定值
    /// </summary>
    public class UrlTokenAuthenticationDefaults
    {
        /// <summary>
        /// 提供固定名称
        /// </summary>
        public const string AuthenticationScheme = "UrlTokenScheme";
    }
}
