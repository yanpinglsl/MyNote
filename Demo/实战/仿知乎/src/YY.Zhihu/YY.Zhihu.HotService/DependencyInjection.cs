using Microsoft.Extensions.DependencyInjection;
using YY.Zhihu.HotService.Core;
using YY.Zhihu.HotService.Data;

namespace YY.Zhihu.HotService
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddHotService(this IServiceCollection services)
        {
            services.AddSingleton<QuestionStatManager>();

            services.AddTransient<QuestionStatQuery>();
            services.AddTransient<HotRankManager>();

            return services;
        }
    }

}
