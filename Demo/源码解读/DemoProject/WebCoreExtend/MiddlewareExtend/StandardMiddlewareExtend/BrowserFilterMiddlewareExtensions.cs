using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCoreExtend.MiddlewareExtend.StandardMiddlewareExtend
{
    public static class BrowserFilterMiddlewareExtensions
    {
        public static IApplicationBuilder UseBrowserFilter(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            return app.UseMiddleware<BrowserFilterMiddleware>();//这里可以给Middleware做些信息信息传递
        }
    }
}
