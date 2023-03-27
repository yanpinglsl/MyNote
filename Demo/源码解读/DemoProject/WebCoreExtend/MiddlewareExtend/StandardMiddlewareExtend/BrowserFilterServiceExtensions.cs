using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCoreExtend.MiddlewareExtend.StandardMiddlewareExtend
{
    public static class BrowserFilterServiceExtensions
    {
        /// <summary>
        /// IOC注册
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddBrowserFilter(this IServiceCollection services)
        {
            return services.AddSingleton<IBrowserCheck, BrowserCheckService>();
            //没有option---就用默认的
        }

        public static IServiceCollection AddBrowserFilter(this IServiceCollection services, Action<BrowserFilterOptions> configure)
        {
            services.Configure(configure);//这个是之前讲的Options,只是配置，但是生效是在访问Value属性时
            return services.AddBrowserFilter();
        }
    }
}
