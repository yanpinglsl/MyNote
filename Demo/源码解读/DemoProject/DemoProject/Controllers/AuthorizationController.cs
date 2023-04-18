using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DemoProject.Controllers
{
    //[Authorize(AuthenticationSchemes = "Cookie", Roles = "Admin,User", Policy = "Custom,Eleven")]

    /// <summary>
    /// dotnet run --urls="http://*:5726" ip="127.0.0.1" /port=5726 ConnectionStrings:Write=CommandLineArgument
    /// </summary>
    public class AuthorizationController : Controller
    {
        private readonly IConfiguration _iConfiguration = null;
        private readonly ILogger<AuthorizationController> _logger;

        public AuthorizationController(IConfiguration configuration
            , ILogger<AuthorizationController> logger)
        {
            this._iConfiguration = configuration;
            this._logger = logger;
        }


        [AllowAnonymous]
        public IActionResult Index()
        {
            Console.WriteLine($"This is {nameof(AuthorizationController)} Index");
            return View();
        }

        /// <summary>
        /// http://localhost:5726/Authorization/Login?name=Eleven&password=123456&role=Admin
        /// http://localhost:5726/Authorization/Login?name=Eleven&password=123456&role=User
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<IActionResult> Login(string name, string password, string role = "Admin")
        {
            if ("Eleven".Equals(name, StringComparison.CurrentCultureIgnoreCase)
                && password.Equals("123456"))
            {
                var claimIdentity = new ClaimsIdentity("Custom");
                claimIdentity.AddClaim(new Claim(ClaimTypes.Name, name));
                //claimIdentity.AddClaim(new Claim(ClaimTypes.Email, "xuyang@ZhaoxiEdu.Net"));
                claimIdentity.AddClaim(new Claim(ClaimTypes.Email, "57265177@qq.com"));
                claimIdentity.AddClaim(new Claim(ClaimTypes.Role, role));

                await base.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimIdentity), new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(30),
                });//登录为默认的scheme  cookies
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

        [AllowAnonymous]
        public async Task<IActionResult> LoginPath()
        {
            await Task.CompletedTask;
            return new JsonResult(new
            {
                Result = false,
                Message = "Redirect to LoginPath"
            });
        }

        [AllowAnonymous]
        public async Task<IActionResult> AccessDeniedPath()
        {
            await Task.CompletedTask;
            return new JsonResult(new
            {
                Result = false,
                Message = "Redirect to AccessDeniedPath"
            });
        }

        [AllowAnonymous]
        public async Task<IActionResult> ForbidPath()
        {
            await Task.CompletedTask;
            return new JsonResult(new
            {
                Result = false,
                Message = "Redirect to ForbidPath"
            });
        }

        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await base.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return new JsonResult(new
            {
                Result = true,
                Message = "退出成功"
            });
        }

        /// <summary>
        /// 需要授权的页面
        /// http://localhost:5726/Authorization/Info
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = "Cookies")]//
        //[Authorize]//表示需要授权，没有任何规则，只要求有用户信息
        public IActionResult Info()
        {
            this._logger.LogWarning("This is Authorization-Info 1");
            return View();
        }

        /// <summary>
        /// http://localhost:5726/Authorization/InfoAdmin
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = "Cookies", Roles = "Admin")]
        public IActionResult InfoAdmin()
        {
            return View();
        }
        /// <summary>
        /// http://localhost:5726/Authorization/InfoUser
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = "Cookies", Roles = "User")]
        public IActionResult InfoUser()
        {
            return View();
        }
        /// <summary>
        /// http://localhost:5726/Authorization/InfoAdminUser
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = "Cookies", Roles = "Admin,User")]
        public IActionResult InfoAdminUser()
        {
            return View();
        }

        #region Policy

        #region 1
        /// <summary>
        /// http://localhost:5726/Authorization/InfoAdminPolicy
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = "Cookies", Policy = "AdminPolicy")]
        public IActionResult InfoAdminPolicy()
        {
            this._logger.LogWarning("This is Authorization-InfoAdminPolicy 1");
            var endpoint = base.HttpContext.GetEndpoint();
            base.ViewBag.Info = $"endpoint:{endpoint.DisplayName}";

            return View();
        }

        /// <summary>
        /// http://localhost:5726/Authorization/InfoUserPolicy
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = "Cookies", Policy = "UserPolicy")]
        public IActionResult InfoUserPolicy()
        {
            this._logger.LogWarning("This is Authorization-InfoUserPolicy 1");
            var endpoint = base.HttpContext.GetEndpoint();
            base.ViewBag.Info = $"endpoint:{endpoint.DisplayName}";

            return View();
        }
        /// <summary>
        /// http://localhost:5726/Authorization/InfoUserAdminPolicy
        /// 报错，不存在
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = "Cookies", Policy = "AdminPolicy,UserPolicy")]
        public IActionResult InfoUserAdminPolicy()
        {
            this._logger.LogWarning("This is Authorization-InfoUserAdminPolicy 1");
            var endpoint = base.HttpContext.GetEndpoint();
            base.ViewBag.Info = $"endpoint:{endpoint.DisplayName}";

            return View();
        }
        #endregion


        #region 2
        /// <summary>
        /// http://localhost:5726/Authorization/InfoAdminQQEmail
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = "Cookies", Roles = "Admin", Policy = "QQEmail")]
        public IActionResult InfoAdminQQEmail()
        {
            this._logger.LogWarning("This is Authorization-InfoAdminQQEmail 1");
            var endpoint = base.HttpContext.GetEndpoint();
            base.ViewBag.Info = $"endpoint:{endpoint.DisplayName}";

            return View();
        }

        /// <summary>
        /// http://localhost:5726/Authorization/InfoUserQQEmail
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = "Cookies", Roles = "User", Policy = "QQEmail")]
        public IActionResult InfoUserQQEmail()
        {
            this._logger.LogWarning("This is Authorization-InfoUserQQEmail 1");
            var endpoint = base.HttpContext.GetEndpoint();
            base.ViewBag.Info = $"endpoint:{endpoint.DisplayName}";

            return View();
        }

        /// <summary>
        /// http://localhost:5726/Authorization/InfoAdminZhaoxiEmail
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = "Cookies", Roles = "Admin", Policy = "ZhaoxiEmail")]
        public IActionResult InfoAdminZhaoxiEmail()
        {
            this._logger.LogWarning("This is Authorization-InfoQQEmail 1");
            var endpoint = base.HttpContext.GetEndpoint();
            base.ViewBag.Info = $"endpoint:{endpoint.DisplayName}";

            return View();
        }
        #endregion


        #region 3
        /// <summary>
        /// http://localhost:5726/Authorization/InfoMutiSchemeAll   失败
        /// http://localhost:5726/Authorization/InfoMutiSchemeAll?UrlToken=eleven-123456  成功
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = "Cookies,UrlTokenScheme", Roles = "User", Policy = "QQEmail")]
        public IActionResult InfoMutiSchemeAll()
        {
            this._logger.LogWarning("This is Authorization-InfoMutiSchemeAll 1");
            var endpoint = base.HttpContext.GetEndpoint();
            base.ViewBag.Info = $"endpoint:{endpoint.DisplayName}";

            return View();
        }
        #endregion

        [Authorize(AuthenticationSchemes = "Cookies", Policy = "QQEmail")]
        public IActionResult InfoQQEmail()
        {
            this._logger.LogWarning("This is Authorization-InfoQQEmail 1");
            var endpoint = base.HttpContext.GetEndpoint();
            base.ViewBag.Info = $"endpoint:{endpoint.DisplayName}";

            return View();
        }

        [Authorize(AuthenticationSchemes = "Cookies", Policy = "DoubleEmail")]
        public IActionResult InfoDoubleEmail()
        {
            this._logger.LogWarning("This is Authorization-DoubleEmail 1");
            var endpoint = base.HttpContext.GetEndpoint();
            base.ViewBag.Info = $"endpoint:{endpoint.DisplayName}";

            return View();
        }
        #endregion

        #region 4
        /// <summary>
        /// http://localhost:5726/Authorization/InfoMutiAuthorize?UrlToken=eleven-123456  成功
        /// 
        /// http://localhost:5726/Authorization/InfoMutiAuthorize   失败
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = "Cookies", Policy = "QQEmail", Roles = "Admin,User")]
        [Authorize(AuthenticationSchemes = "UrlTokenScheme", Policy = "UserPolicy", Roles = "Admin,User")]
        public IActionResult InfoMutiAuthorize()
        {
            this._logger.LogWarning("This is Authorization-InfoMutiAuthorize 1");
            var endpoint = base.HttpContext.GetEndpoint();
            base.ViewBag.Info = $"endpoint:{endpoint.DisplayName}";

            return View();
        }

        /// <summary>
        /// http://localhost:5726/Authorization/InfoMutiAuthorizeOneFaild?UrlToken=eleven-123456  失败
        /// 应该失败，但是成功了，why？  ToDo
        /// Cookie的Role是错的
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = "Cookies", Policy = "QQEmail", Roles = "User")]
        [Authorize(AuthenticationSchemes = "UrlTokenScheme", Policy = "UserPolicy", Roles = "User")]
        public IActionResult InfoMutiAuthorizeOneFaild()
        {
            this._logger.LogWarning("This is Authorization-InfoMutiAuthorizeOneFaild 1");
            var endpoint = base.HttpContext.GetEndpoint();
            base.ViewBag.Info = $"endpoint:{endpoint.DisplayName}";

            return View();
        }
        #endregion

        #region 5

        /// <summary>
        /// http://localhost:5726/Authorization/InfoMutiScheme
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = "Cookies", Policy = "MutiPolicy", Roles = "Admin,User")]
        public IActionResult InfoMutiScheme()
        {
            this._logger.LogWarning("This is Authorization-InfoMutiScheme 1");
            var endpoint = base.HttpContext.GetEndpoint();
            base.ViewBag.Info = $"endpoint:{endpoint.DisplayName}";

            return View();
        }
        #endregion

    }
}
