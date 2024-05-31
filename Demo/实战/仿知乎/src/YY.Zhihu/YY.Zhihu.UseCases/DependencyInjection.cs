using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace YY.Zhihu.UseCases
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUseCaseServices(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddMediatR(cfg => { cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()); });

            return services;
        }
    }
}