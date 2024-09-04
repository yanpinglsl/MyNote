

using Consul;

namespace YY.Common.Consul.ServiceDiscovery
{
    public class ConsulServiceClient(IConsulClient consulClient) : IServiceClient
    {
        public async Task<List<string>> GetServicesAsync(string serviceName)
        {
            var queryResult = await consulClient.Health.Service(serviceName, null, true);

            return queryResult.Response
                .Select(serviceEntry => serviceEntry.Service.Address + ":" + serviceEntry.Service.Port)
                .ToList();
        }
    }


}
