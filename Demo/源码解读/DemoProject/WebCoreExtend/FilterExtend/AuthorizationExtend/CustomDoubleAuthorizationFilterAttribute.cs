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

namespace WebCoreExtend.FilterExtend.AuthorizationExtend
{
    /// <summary>
    /// 自定义授权Filter
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class CustomDoubleAuthorizationFilterAttribute : Attribute, IAuthorizationFilter, IAsyncAuthorizationFilter, IFilterMetadata, IOrderedFilter
    {
        private int _Order = 0;

        public int Order
        {
            get { return _Order; }
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            Console.WriteLine($"This is {nameof(CustomDoubleAuthorizationFilterAttribute)} .OnAuthorization");
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            Console.WriteLine($"This is {nameof(CustomDoubleAuthorizationFilterAttribute)} .OnAuthorizationAsync");
            await Task.CompletedTask;
        }
    }


    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class CustomShowAuthorizationFilterAttribute : Attribute, IAuthorizationFilter, IFilterMetadata, IOrderedFilter
    {
        private int _Order = 0;

        public int Order
        {
            get { return _Order; }
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            Console.WriteLine($"This is {nameof(CustomShowAuthorizationFilterAttribute)} .OnAuthorization");
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class CustomShowAsyncAuthorizationFilterAttribute : Attribute, IAsyncAuthorizationFilter, IFilterMetadata, IOrderedFilter
    {
        private int _Order = 0;

        public int Order
        {
            get { return _Order; }
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            Console.WriteLine($"This is {nameof(CustomShowAsyncAuthorizationFilterAttribute)} .OnAuthorizationAsync");
            string user = context.HttpContext.Request.Query["UserName"];

            if (string.IsNullOrEmpty(user))
            {
                context.Result = new RedirectResult("~/Home/Index");
            }
            else
            {
                Console.WriteLine($"This is {user}访问系统");
            }
            await Task.CompletedTask;
        }
    }
}
