
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace WebCoreExtend.RouteExtend.DynamicRouteExtend
{
    /// <summary>
    /// 
    /// </summary>
    public static class CustomRouteExtensions
    {
        #region DynamicRoute  
        /// <summary>
        /// 需要提供数据获取
        /// </summary>
        /// <param name="services"></param>
        public static void AddDynamicRoute(this IServiceCollection services)
        {
            services.AddSingleton<TranslationTransformer>();
            services.AddSingleton<CustomTranslationSource>();
        }
        /// <summary>
        /// 需要配置路由
        /// 满足这个规则，触发动态路由--再把值做映射
        /// </summary>
        /// <param name="endpoints"></param>
        public static void UseDynamicRouteDefault(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapDynamicControllerRoute<TranslationTransformer>("{language}/{controller}/{action}");
        }
        #endregion


    }
}
