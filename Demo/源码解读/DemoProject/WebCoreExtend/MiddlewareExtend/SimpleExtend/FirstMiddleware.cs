using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCoreExtend.MiddlewareExtend.SimpleExtend
{
    public class FirstMiddleware//不实现接口
    {
        private readonly RequestDelegate _next;
        /// <summary>
        /// 一定有个构造函数，传递RequestDelegate
        /// </summary>
        /// <param name="next"></param>
        public FirstMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// 方法名字叫 InvokeAsync 或者Invoke
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            await context.Response.WriteAsync($"{nameof(FirstMiddleware)},Hello World1 Start!<br/>");
            await _next(context);
            await context.Response.WriteAsync($"{nameof(FirstMiddleware)},Hello World1  End!<br/>");

        }
    }
}
