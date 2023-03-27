using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebCoreExtend.MiddlewareExtend.StandardMiddlewareExtend;

namespace WebCoreExtend.MiddlewareExtend
{
    /// <summary>
    /// 自定义异常中间件
    /// </summary>
    public class CustomerExceptionMiddleware
    {
        /// <summary>
        /// 委托
        /// </summary>
        private readonly RequestDelegate _next;

        public CustomerExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {

                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync("Exception:" + ex.Message);
            }
        }
    }
    public static class ExceptionMiddlewareExtension
    {
        public static IApplicationBuilder UseCustomException(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            return app.UseMiddleware<CustomerExceptionMiddleware>();
        }
    }
}
