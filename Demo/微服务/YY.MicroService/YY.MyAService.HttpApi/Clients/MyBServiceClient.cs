using YY.Common.Consul.ServiceDiscovery;
using YY.LoadBalancer;
using YY.LoadBalancer.AspNetCore;
using YY.Common.Polly;

namespace YY.MyAService.HttpApi.Clients
{
    public static class MyBServiceClientExtension
    {
        public static void AddMyBServiceClient(this IServiceCollection services)
        {
            services.AddLoadBalancer<MyBServiceClient>(LoadBalancingStrategy.RoundRobin);
            services.AddHttpClient<MyBServiceClient>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(10);
            }).AddCustomResiliencePipeline();
        }
    }

    public class MyBServiceClient
    {
        private const string ServiceName = "YY.MyBService.HttpApi";

        private readonly HttpClient _httpClient;

        public MyBServiceClient(IServiceClient serviceClient,
            ILoadBalancer<MyBServiceClient> loadBalancer,
            HttpClient httpClient,
            ILogger<MyBServiceClient> logger)
        {
            _httpClient = httpClient;

            //var serviceList = serviceClient.GetServicesAsync(ServiceName).Result;
            //测试
            var serviceList = new List<string>()
            {
                "127.0.0.1:5800",
                "127.0.0.1:5801",
                "127.0.0.1:5802"
            };
            var serviceAddress = loadBalancer.GetNode(serviceList);
            logger.LogInformation(serviceAddress);

            _httpClient.BaseAddress = new Uri($"http://{serviceAddress}");
        }

        public async Task<string> GetHelloAsync()
        {
            var response = await _httpClient.GetAsync("api/Hello");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }

    //public class MyBServiceClient(IServiceClient serviceClient, IHttpClientFactory httpClientFactory, ILogger<MyBServiceClient> logger)
    //{
    //    private const string ServiceName = "YY.MyBService.HttpApi";

    //    private readonly ILoadBalancer _loadBalancer = new LoadBalancer.LoadBalancer(LoadBalancingStrategy.Random);

    //    public async Task<string> GetHelloAsync()
    //    {
    //        var serviceList = await serviceClient.GetServicesAsync(ServiceName);
    //        var service = _loadBalancer.GetNode(serviceList);
    //        logger.LogInformation(service);
    //        var httpClient = httpClientFactory.CreateClient(nameof(MyBServiceClient));
    //        Console.WriteLine(httpClient.Timeout);
    //        httpClient.BaseAddress = new Uri($"http://{service}");
    //        var response = await httpClient.GetAsync("api/Hello");
    //        response.EnsureSuccessStatusCode();
    //        return await response.Content.ReadAsStringAsync();
    //    }
    //}

}