using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCoreExtend.CommonModels;
using WebCoreExtend.WebCore;

namespace WebCoreExtend.FilterExtend.AuthorizationExtend
{
    /// <summary>
    /// 自定义授权Filter
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class CustomAuthorizationFilterAttribute : Attribute, IAuthorizationFilter, IFilterMetadata, IOrderedFilter
    {
        #region Identity
        //private readonly ILogger<CustomAuthorizationFilterAttribute> _logger;
        //public CustomAuthorizationFilterAttribute(ILogger<CustomAuthorizationFilterAttribute> logger)
        //{
        //    this._logger = logger;
        //}
        #endregion

        public int Order
        {
            get; set;
        } = 0;
        /// <summary>
        /// 发生在请求刚进入MVC流程，还没实例化控制器
        /// 检测用户登陆--以及是否有权限
        /// 
        /// ASP.NET Core已经鉴权授权，其实这个不怎么用了
        /// </summary>
        /// <param name="context"></param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            Console.WriteLine($"This is {nameof(CustomAuthorizationFilterAttribute)} .OnAuthorization");

            //throw new Exception("CustomAuthorizationFilter Exception");
            if (context.Filters.Any(item => item is IAllowAnonymousFilter))//Microsoft.AspNetCore.Mvc.Authorization.AllowAnonymousFilter
            {
                return;//匿名特性AllowAnonymousAttribute 会生成Filter
            }
            if (context.ActionDescriptor.EndpointMetadata.Any(item => item is IAllowAnonymous))//鉴权授权的Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute
            {
                return;//匿名就不检测，直接继续
            }
            string user = context.HttpContext.Request.Query["UserName"];
            //var memberValidation = HttpContext.Current.Request.Cookies.Get("CurrentUser");//使用cookie
            //也可以使用数据库、nosql等介质
            //context.HttpContext.GetCurrentUserBySession();//读取用户信息
            if (string.IsNullOrEmpty(user))//这里可以根据context.HttpContext.Request.Path判断有没有权限
            {
                if (context.HttpContext.Request.IsAjaxRequest())
                {
                    context.Result = new JsonResult(new AjaxResult()
                    {
                        Result = false,
                        StatusCode = 401,
                        Message = "用户未登录",
                    });//断路器
                }
                else
                {
                    //context.HttpContext.Session.SetString("CurrentUrl", context.HttpContext.Request.Path.Value);
                    context.Result = new RedirectResult("~/Filter/Login");
                }
            }
            else
            {
                Console.WriteLine($"This is {user}访问系统");
                //this._logger.LogDebug($"{user} 访问系统");
            }
        }
    }
}
