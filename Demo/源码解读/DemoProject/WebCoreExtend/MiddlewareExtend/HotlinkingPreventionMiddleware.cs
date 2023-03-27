using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCoreExtend.MiddlewareExtend
{
    public class HotlinkingPreventionMiddleware
    {
        private readonly string _wwwrootFolder;
        private readonly RequestDelegate _next;

        public HotlinkingPreventionMiddleware(RequestDelegate next, IHostingEnvironment env)
        {
            _wwwrootFolder = env.WebRootPath;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var applicationUrl = $"{context.Request.Scheme}://{context.Request.Host.Value}";
            var headersDictionary = context.Request.Headers;
            var urlReferrer = headersDictionary[HeaderNames.Referer].ToString();
            string url = context.Request.Path.Value;
            if (!url.Contains(".jpg"))
            {
                await _next(context);//走正常流程，啥事儿不干
            }
            else
            {
                if (string.IsNullOrEmpty(urlReferrer))  //直接访问
                {
                    await this.SetForbiddenImage(context);
                }
                else if (!urlReferrer.StartsWith(applicationUrl)) //非当前域名
                {
                    await this.SetForbiddenImage(context);
                }
                else
                {
                    await _next(context);
                }
            }

        }

        private async Task SetForbiddenImage(HttpContext context)
        {
            var unauthorizedImagePath = Path.Combine(_wwwrootFolder, "Images/Forbidden.jpg");

            await context.Response.SendFileAsync(unauthorizedImagePath);
        }

    }

    public static class HotlinkingPreventionExtensions
    {
        public static IApplicationBuilder UseHotlinkingPreventionMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<HotlinkingPreventionMiddleware>();
        }
    }
}
