using Microsoft.Extensions.DependencyInjection;

namespace YY.Common.Consul.ServiceDiscovery
{
    public static class ConsulServiceDiscoveryExtension
    {
        public static IServiceCollection AddConsulClient(this IServiceCollection services)
        {
            services.AddSingleton<IServiceClient, ConsulServiceClient>();
            return services;
        }
    }


}
