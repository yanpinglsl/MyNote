using Bogus.DataSets;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.Application.Services;
using YY.Zhihu.UseCases.Common.Interfaces;

namespace YY.Zhihu.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWebServices(this IServiceCollection services)
        {
            services.AddScoped<IUser,CurrentUser>();
            services.AddHttpContextAccessor();
            return services;
        }
    }
}
