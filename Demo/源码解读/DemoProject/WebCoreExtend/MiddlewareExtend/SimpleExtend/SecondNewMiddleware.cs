using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCoreExtend.MiddlewareExtend.SimpleExtend
{
    public class SecondNewMiddleware : IMiddleware, IDisposable
    {
        private readonly ILogger _logger;

        public SecondNewMiddleware(ILogger<SecondNewMiddleware> logger)
        {
            this._logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            this._logger.LogWarning($"{nameof(SecondNewMiddleware)},Hello World 2 New!<br/>");

            await context.Response.WriteAsync($"{nameof(SecondNewMiddleware)},Hello World2 New Start!<br/>");
            await next(context);
            await context.Response.WriteAsync($"{nameof(SecondNewMiddleware)},Hello World2 New End!<br/>");
        }
        public void Dispose()
        {
            Console.WriteLine($"{nameof(SecondNewMiddleware)} 释放需要释放的资源");
        }
    }
}
