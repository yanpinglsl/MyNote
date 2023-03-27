using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCoreExtend.MiddlewareExtend
{
    public class HeaderReadWriteMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<HeaderReadWriteMiddleware> _logger;

        public HeaderReadWriteMiddleware(RequestDelegate next, ILogger<HeaderReadWriteMiddleware> logger)
        {
            this._next = next;
            this._logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //await context.Response.WriteAsync("This is Hello World 1 start");
            //context.Response.OnStarting(state =>
            //{
            //    var httpContext = (HttpContext)state;
            //    httpContext.Response.Headers.Add("middleware", "HeaderReadMiddleware12345");
            //    Console.WriteLine(httpContext.Response.StatusCode);
            //    return Task.CompletedTask;
            //}, context);

            context.Response.OnStarting(async state =>
            {
                var httpContext = (HttpContext)state;
                httpContext.Response.Headers.Add("middlewareStarting", "HeaderReadWriteMiddleware12345");
                //await httpContext.Response.WriteAsync("This is Eleven"); //写不进去，带上中间件，写入时才发生的
                await Task.CompletedTask;
            }, context);

            context.Response.OnCompleted(async state =>
            {
                var httpContext = (HttpContext)state;
                Console.WriteLine($"请求结果StatusCode={httpContext.Response.StatusCode}");
                httpContext.Response.Headers.Add("middlewareComplated", "HeaderReadWriteMiddleware12345222");//写不进去了，不生效
                //主要是用来做资源释放，或者通知
                await Task.CompletedTask;
            }, context);

            await this._next.Invoke(context);
            //await context.Response.WriteAsync("This is Hello World 1   end");

        }
    }
}
