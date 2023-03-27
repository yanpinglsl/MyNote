using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCoreExtend.StartupExtend
{
    public class CustomStartupFilter : IStartupFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nextBuild">其实就是后面的默认中间件组装</param>
        /// <returns></returns>
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> nextBuild)
        {
            return new Action<IApplicationBuilder>(
            app =>
            {
                app.Use(next =>
                {
                    Console.WriteLine($"This is {nameof(CustomStartupFilter)} middleware xxxxx");
                    return new RequestDelegate(
                        async context =>
                        {
                            context.Response.ContentType = "text/html";                            
                            await context.Response.WriteAsync($"This is {nameof(CustomStartupFilter)} Hello World xxxxx start </br>");
                            await next.Invoke(context);
                            await context.Response.WriteAsync($"This is {nameof(CustomStartupFilter)} Hello World xxxxx   end </br>");
                        });
                });
                nextBuild.Invoke(app);
            }
           );
        }
    }
}
