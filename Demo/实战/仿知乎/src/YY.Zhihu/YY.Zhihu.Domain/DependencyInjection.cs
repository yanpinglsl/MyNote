using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.Domain.Interfaces;
using YY.Zhihu.Domain.Services;

namespace YY.Zhihu.Domain
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddScoped<IAppUserService, AppUserService>();
            return services;
        }
    }
}
