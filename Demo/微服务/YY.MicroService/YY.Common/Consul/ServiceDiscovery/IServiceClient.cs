namespace YY.Common.Consul.ServiceDiscovery
{
    public interface IServiceClient
    {
        Task<List<string>> GetServicesAsync(string serviceName);

    }


}
