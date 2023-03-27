using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;
using WebCoreExtend.AuthenticationExtend;

namespace DemoProject.Controllers
{
    /// <summary>
    /// 鉴权专用
    /// dotnet run --urls="http://*:5726" ip="127.0.0.1" /port=5726 ConnectionStrings:Write=CommandLineArgument
    /// </summary>
    public class AuthenticationController : Controller
    {
        private readonly IConfiguration _iConfiguration = null;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(IConfiguration configuration
            , ILogger<AuthenticationController> logger)
        {
            this._iConfiguration = configuration;
            this._logger = logger;
        }

        /// <summary>
        /// 首页
        /// http://localhost:5726/Authentication/Index
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            this._logger.LogWarning("This is AuthenticationController-Index 1");
            return View();
        }

        #region 基于Cookie基本鉴权-授权基本流程
        /// <summary>
        /// http://localhost:5726/Authentication/Login?name=Eleven&password=123456
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<IActionResult> Login(string name, string password)
        {
            if ("Eleven".Equals(name, StringComparison.CurrentCultureIgnoreCase)
                && password.Equals("123456"))
            {
                var claimIdentity = new ClaimsIdentity("Custom");
                claimIdentity.AddClaim(new Claim(ClaimTypes.Name, name));
                claimIdentity.AddClaim(new Claim(ClaimTypes.Email, "57265177@qq.com"));
                claimIdentity.AddClaim(new Claim("Email", "12345678@163.com"));
                claimIdentity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
                //claimIdentity.AddClaim(new Claim(ClaimTypes.Role, "User"));
                claimIdentity.AddClaim(new Claim(ClaimTypes.Country, "Chinese"));
                claimIdentity.AddClaim(new Claim(ClaimTypes.DateOfBirth, "1986"));

                //把用户信息装到ClaimsPrincipal
                var claimsPrincipal = new ClaimsPrincipal(claimIdentity);
                //登录，把用户信息写入到cookie
                await base.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(30),
                });//登陆默认Scheme，写入Cookie
                return new JsonResult(new
                {
                    Result = true,
                    Message = "登录成功"
                });
            }
            else
            {
                await Task.CompletedTask;
                return new JsonResult(new
                {
                    Result = false,
                    Message = "登录失败"
                });
            }
        }

        /// <summary>
        /// 需要授权的页面
        /// http://localhost:5726/Authentication/InfoWithAuthorize
        /// </summary>
        /// <returns></returns>
        [Authorize]//需要鉴权+授权--
        public IActionResult InfoWithAuthorize()
        {
            this._logger.LogWarning("This is Authentication-InfoWithAuthorize 1");
            return View();
        }


        /// <summary>
        /// 退出登陆
        /// http://localhost:5726/Authentication/Logout
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Logout()
        {
            await base.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);//cookie退出，其实就是清除cookie
            return new JsonResult(new
            {
                Result = true,
                Message = "退出成功"
            });
        }

        /// <summary>
        /// 角色授权
        /// http://localhost:5726/Authentication/InfoWithRolesAdmin
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]//要求Roles必须是Admin

        //[Authorize(Roles = "Admin,User")]//满足一个就行
        public IActionResult InfoWithRolesAdmin()
        {
            this._logger.LogWarning("This is Authentication-InfoWithRolesAdmin 1");
            return View();
        }

        /// <summary>
        /// 方案授权
        /// http://localhost:5726/Authentication/InfoWithPolicy
        /// </summary>
        /// <returns></returns>
        //[Authorize(Policy = "AdminPolicy", AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]//等价于Roles = "Admin"
        [Authorize(Policy = "MutiPolicy", AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public IActionResult InfoWithPolicy()
        {
            this._logger.LogWarning("This is Authentication-InfoWithPolicy 1");
            return View();
        }

        /// <summary>
        /// 无需权限验证
        /// http://localhost:5726/Authentication/InfoWithAllowAnonymous
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult InfoWithAllowAnonymous()
        {
            this._logger.LogWarning("This is Authentication-InfoWithAllowAnonymous 1");
            return View();
        }

        /// <summary>
        /// 主动鉴权
        /// http://localhost:5726/Authentication/Authentication
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<IActionResult> Authentication()
        {
            //Console.WriteLine($"base.HttpContext.User?.Claims?.First()?.Value={base.HttpContext.User?.Claims?.First()?.Value}");

            var result = await base.HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (result?.Principal != null)
            {
                base.HttpContext.User = result.Principal;
                return new JsonResult(new
                {
                    Result = true,
                    Message = $"认证成功，包含用户{base.HttpContext.User.Identity.Name}"
                });
            }
            else
            {
                return new JsonResult(new
                {
                    Result = false,
                    Message = $"认证失败，用户未登录"
                });
            }
        }

        /// <summary>
        /// 主动授权-名字是Eleven
        /// http://localhost:5726/Authentication/Authorization
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Authorization()
        {
            var result = await base.HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (result?.Principal == null)
            {
                return new JsonResult(new
                {
                    Result = true,
                    Message = $"认证失败，用户未登录"
                });
            }
            else
            {
                base.HttpContext.User = result.Principal;
            }

            //授权
            var user = base.HttpContext.User;
            if (user?.Identity?.IsAuthenticated ?? false)
            {
                if (!user.Identity.Name.Equals("Eleven", StringComparison.OrdinalIgnoreCase))
                {
                    await base.HttpContext.ForbidAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    return new JsonResult(new
                    {
                        Result = false,
                        Message = $"授权失败，用户{base.HttpContext.User.Identity.Name}没有权限"
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        Result = false,
                        Message = $"授权成功，用户{base.HttpContext.User.Identity.Name}具备权限"
                    });
                }
            }
            else
            {
                await base.HttpContext.ChallengeAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return new JsonResult(new
                {
                    Result = false,
                    Message = $"授权失败，没有登录"
                });
            }
        }
        #endregion

        #region 自定义UrlToken

        /// <summary>
        /// http://localhost:5726/Authentication/UrlToken
        /// http://localhost:5726/Authentication/UrlToken?UrlToken=eleven-123456
        /// </summary>
        /// <returns></returns>
        //[Authorize(AuthenticationSchemes= UrlTokenAuthenticationDefaults.AuthenticationScheme)]
        [Authorize]
        public async Task<IActionResult> UrlToken()
        {
            Console.WriteLine($"请求进入UrlToken，表明鉴权授权已通过");
            Console.WriteLine($"当然进行主动鉴权");

            var result = await base.HttpContext.AuthenticateAsync(UrlTokenAuthenticationDefaults.AuthenticationScheme);

            if (result?.Principal == null)//不会发生
            {
                return new JsonResult(new
                {
                    Result = true,
                    Message = $"认证失败，用户未登录"
                });
            }
            else
            {
                base.HttpContext.User = result.Principal;
            }

            //主动授权检测
            var user = base.HttpContext.User;
            if (user?.Identity?.IsAuthenticated ?? false)
            {
                if (!user.Identity.Name.Equals("eleven-123456", StringComparison.OrdinalIgnoreCase))
                {
                    await base.HttpContext.ForbidAsync(UrlTokenAuthenticationDefaults.AuthenticationScheme);
                    return new JsonResult(new
                    {
                        Result = false,
                        Message = $"授权失败，用户{base.HttpContext.User.Identity.Name}没有权限"
                    });
                }
                else
                {
                    //有权限
                    return new JsonResult(new
                    {
                        Result = true,
                        Message = $"授权成功，正常访问页面！",
                        Html = "Hello Root!"
                    });
                }
            }
            else
            {
                await base.HttpContext.ChallengeAsync(UrlTokenAuthenticationDefaults.AuthenticationScheme);
                return new JsonResult(new
                {
                    Result = false,
                    Message = $"授权失败，没有登录"
                });
            }
        }


        /// <summary>
        /// http://localhost:5726/Authentication/AuthorizeData
        /// http://localhost:5726/Authentication/AuthorizeData?UrlToken=eleven-123456
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> AuthorizeData()
        {
            await base.HttpContext.SignOutAsync("UrlTokenScheme");
            return new JsonResult(new
            {
                Result = true,
                Message = "退出成功"
            });
        }
        #endregion


        #region 多Scheme：UrlToken+Cookie
        /// <summary>
        /// http://localhost:5726/Authentication/UrlCookieByDefault
        /// http://localhost:5726/Authentication/UrlCookieByDefault?UrlToken=eleven-123456
        /// </summary>
        /// <returns></returns>
        [Authorize()]//为空，则是默认(UrlToken)--甚至可以不要
        public async Task<IActionResult> UrlCookieByDefault()
        {
            Console.WriteLine($"主动鉴权之前：base.HttpContext.User?.Claims?.First()?.Value == null?{base.HttpContext.User?.Claims?.First()?.Value == null}");
            Console.WriteLine($"主动鉴权之前：base.HttpContext.User?.Identities?.Count() ={base.HttpContext.User?.Identities?.Count()}");


            var urlToken = await base.HttpContext.AuthenticateAsync(UrlTokenAuthenticationDefaults.AuthenticationScheme);
            Console.WriteLine($"urlToken?.Principal == null ={urlToken?.Principal == null}");


            var cookie = await base.HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Console.WriteLine($"urlToken?.Principal == null ={cookie?.Principal == null}");

            return new JsonResult(new
            {
                Result = true,
                Message = $"认证成功，有用户信息"
            });
        }

        /// <summary>
        /// http://localhost:5726/Authentication/UrlCookieByUrlToken
        /// http://localhost:5726/Authentication/UrlCookieByUrlToken?UrlToken=eleven-123456
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = UrlTokenAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UrlCookieByUrlToken()
        {
            Console.WriteLine($"主动鉴权之前：base.HttpContext.User?.Claims?.First()?.Value == null?{base.HttpContext.User?.Claims?.First()?.Value == null}");
            Console.WriteLine($"主动鉴权之前：base.HttpContext.User?.Identities?.Count() ={base.HttpContext.User?.Identities?.Count()}");


            var urlToken = await base.HttpContext.AuthenticateAsync(UrlTokenAuthenticationDefaults.AuthenticationScheme);
            Console.WriteLine($"urlToken?.Principal == null ={urlToken?.Principal == null}");


            var cookie = await base.HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Console.WriteLine($"urlToken?.Principal == null ={cookie?.Principal == null}");

            return new JsonResult(new
            {
                Result = true,
                Message = $"认证成功，有用户信息"
            });
        }

        /// <summary>
        /// http://localhost:5726/Authentication/UrlCookieByCookie
        /// http://localhost:5726/Authentication/UrlCookieByCookie?UrlToken=eleven-123456
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UrlCookieByCookie()
        {
            Console.WriteLine($"主动鉴权之前：base.HttpContext.User?.Claims?.First()?.Value == null?{base.HttpContext.User?.Claims?.First()?.Value == null}");
            Console.WriteLine($"主动鉴权之前：base.HttpContext.User?.Identities?.Count() ={base.HttpContext.User?.Identities?.Count()}");


            var urlToken = await base.HttpContext.AuthenticateAsync(UrlTokenAuthenticationDefaults.AuthenticationScheme);
            Console.WriteLine($"urlToken?.Principal == null ={urlToken?.Principal == null}");


            var cookie = await base.HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Console.WriteLine($"urlToken?.Principal == null ={cookie?.Principal == null}");

            return new JsonResult(new
            {
                Result = true,
                Message = $"认证成功，有用户信息"
            });
        }


        /// <summary>
        /// http://localhost:5726/Authentication/UrlCookieByDouble
        /// http://localhost:5726/Authentication/UrlCookieByDouble?UrlToken=eleven-123456
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = $"{CookieAuthenticationDefaults.AuthenticationScheme},{UrlTokenAuthenticationDefaults.AuthenticationScheme}")]//多个Scheme 其实授权时信息可以共享
        public async Task<IActionResult> UrlCookieByDouble()
        {
            Console.WriteLine($"主动鉴权之前：base.HttpContext.User?.Claims?.First()?.Value == null?{base.HttpContext.User?.Claims?.First()?.Value == null}");
            Console.WriteLine($"主动鉴权之前：base.HttpContext.User?.Identities?.Count() ={base.HttpContext.User?.Identities?.Count()}");


            var urlToken = await base.HttpContext.AuthenticateAsync(UrlTokenAuthenticationDefaults.AuthenticationScheme);
            Console.WriteLine($"urlToken?.Principal == null ={urlToken?.Principal == null}");


            var cookie = await base.HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Console.WriteLine($"urlToken?.Principal == null ={cookie?.Principal == null}");

            return new JsonResult(new
            {
                Result = true,
                Message = $"认证成功，有用户信息"
            });
        }
        #endregion

    }
}
