using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCoreExtend.MiddlewareExtend.SimpleExtend
{
    public class SecondMiddleware : IMiddleware
    {
        private readonly ILogger _logger;

        public SecondMiddleware(ILogger<SecondMiddleware> logger)
        {
            this._logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            this._logger.LogWarning($"{nameof(SecondMiddleware)},Hello World1=2!<br/>");
            context.Response.ContentType = "text/html";
            await context.Response.WriteAsync($"{nameof(SecondMiddleware)},Hello World2 Start!<br/>");
            await next(context);
            await context.Response.WriteAsync($"{nameof(SecondMiddleware)},Hello World2 End!<br/>");
        }
    }
}
