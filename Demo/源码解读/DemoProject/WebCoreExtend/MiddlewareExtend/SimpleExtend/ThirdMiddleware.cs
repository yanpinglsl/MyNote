using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCoreExtend.MiddlewareExtend.SimpleExtend
{
    /// <summary>
    /// 没有实现接口---需要IOC注入
    /// </summary>
    public class ThirdMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly string _Message;

        
        public ThirdMiddleware(RequestDelegate next, ILogger<ThirdMiddleware> logger, string message)
        {
            this._next = next;
            this._logger = logger;
            this._Message = message;
        }
        /// <summary>
        /// 1 方法名字Invoke或者InvokeAsync
        /// 2 返回类型必须是Task
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            Console.WriteLine($"{nameof(ThirdMiddleware)}---{this._Message}");
            //context.Response.ContentType = "text/html";

            if (!context.Request.Path.Value.Contains("Eleven"))
            {
                await context.Response.WriteAsync($"{nameof(ThirdMiddleware)}This is Hello World 3 End<br/>");
            }
            else
            {
                await context.Response.WriteAsync($"{nameof(ThirdMiddleware)},Hello World ThreeMiddleWare!<br/>");
                //await _next(context);
                await context.Response.WriteAsync($"{nameof(ThirdMiddleware)},Hello World ThreeMiddleWare!<br/>");
            }
        }
    }
}
