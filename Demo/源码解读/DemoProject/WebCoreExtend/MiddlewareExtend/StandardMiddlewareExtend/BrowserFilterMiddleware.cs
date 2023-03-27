using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCoreExtend.MiddlewareExtend.StandardMiddlewareExtend
{
    public class BrowserFilterMiddleware
    {
        #region Identity
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly IBrowserCheck _iBrowserCheck;

        /// <summary>
        /// IOptions<BrowserFilterOptions> options 有2个来源
        /// 既可以Use的时候去直接传递
        /// 也可以Add是Configure,这里再获取
        /// </summary>
        /// <param name="next"></param>
        /// <param name="logger"></param>
        /// <param name="browserCheck"></param>
        /// <param name="options"></param>
        public BrowserFilterMiddleware(RequestDelegate next, ILogger<BrowserFilterMiddleware> logger, IBrowserCheck browserCheck)
        {
            this._next = next;
            this._logger = logger;
            this._iBrowserCheck = browserCheck;
        }
        #endregion

        public async Task InvokeAsync(HttpContext context)
        {
            var result = this._iBrowserCheck.CheckBrowser(context);
            if (!result.Item1)//检查失败
            {
                Console.WriteLine($"{nameof(BrowserFilterMiddleware)} {result.Item2}");
                await context.Response.WriteAsync($"{nameof(BrowserFilterMiddleware)} {result.Item2}");
            }
            else
            {
                //检查通过，就走默认流程
                Console.WriteLine($"{nameof(BrowserFilterMiddleware)} ok");
                await _next(context);
            }
        }
    }
}
