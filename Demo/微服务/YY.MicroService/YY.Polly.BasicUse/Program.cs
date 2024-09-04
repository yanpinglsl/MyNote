using Consul.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Timeout;
using System.Net;
using YY.Common.Consul.ServiceDiscovery;
using YY.LoadBalancer;


namespace YY.Polly.BasicUse
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MultiPolicyCombine();
            Console.ReadLine();
        }


       /// <summary>
       /// 多策略组合包装测试
       /// </summary>
        static async void MultiPolicyCombine()
        {
            var serviceName = "YY.MyBService.HttpApi";

            var services = new ServiceCollection()
                .AddConsul()
                .AddConsulClient();

            services.AddHttpClient(serviceName);
            
            var serviceProvider = services.BuildServiceProvider();

            var serviceClient = serviceProvider.GetRequiredService<IServiceClient>();
            var loadBalancer = new LoadBalancer.LoadBalancer(LoadBalancingStrategy.RoundRobin);

            // 创建策略器
            var policy = PolicyFactory.CreatePolicy();

            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();

            for (var i = 0; i < 1000; i++)
            {
                Console.WriteLine($"----------------第{i}次请求---YY.MyBService.HttpApi-------------");
                try
                {
                    var serviceList = await serviceClient.GetServicesAsync(serviceName);

                    // 策略器执行业务逻辑
                    policy.Execute(() =>
                    {
                        var serviceAddress = loadBalancer.GetNode(serviceList);
                        Console.WriteLine($"{DateTime.Now} - 正在调用:{serviceAddress}");
                        var httpClient = httpClientFactory.CreateClient(serviceName);
                        httpClient.BaseAddress = new Uri($"http://{serviceAddress}");
                        var result = httpClient.GetStringAsync("api/Hello").Result;
                        Console.WriteLine($"调用结果:{result}");
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Polly基本使用
        /// </summary>
        static void BasicUse()
        {
            #region 定义故障的多种方式
            Policy
                .Handle<ArgumentException>(ex => ex.Message == "Error");

            Policy
                .Handle<HttpRequestException>()
                .Or<ArgumentException>();

            Policy
                .Handle<HttpRequestException>(ex => ex.Message == "Http Error")
                .Or<ArgumentException>(ex => ex.ParamName == "example");

            #endregion

            #region 降级策略
            Policy.HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.NotFound)
                .Fallback(() =>
                {
                    Console.WriteLine("未找到");
                    return new HttpResponseMessage(HttpStatusCode.OK);
                })
                .Execute(() => new HttpResponseMessage(HttpStatusCode.NotFound));

            Policy
                .Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.NotFound);

            #endregion

            #region 重试策略
            Policy
                .Handle<ArgumentException>()
                .Retry();
            //指定重试次数
            Policy
                .Handle<ArgumentException>()
                .Retry(3);
            //一直重试
            Policy
                .Handle<ArgumentException>()
                .RetryForever();

            //重试三次，等待时间分别为1、2、3
            Policy
                .Handle<ArgumentException>()
                .WaitAndRetry(new[]
                {
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(3)
                });

            //重试5次，等待时间分别为2的n次方
            Policy
                .Handle<ArgumentException>()
                .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            //一直重试，等待时间分别为2的n次方
            Policy
                .Handle<Exception>()
                .WaitAndRetryForever(retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
            #endregion


            #region 断路器策略
            Policy
                .Handle<Exception>()
                .CircuitBreaker(2, TimeSpan.FromMinutes(1));

            Policy
                .Handle<Exception>()
                .AdvancedCircuitBreaker(
                    failureThreshold: 0.5,
                    samplingDuration: TimeSpan.FromSeconds(10),
                    minimumThroughput: 8,
                    durationOfBreak: TimeSpan.FromSeconds(30)
                );
            #endregion

            #region 超时策略
            Policy.Timeout(3);
            #endregion

            #region 限流策略
            //该策略将允许在1分钟窗口内最多执行100次请求
            Policy.RateLimit(100, TimeSpan.FromMinutes(1));
            #endregion

            #region 舱壁隔离策略
            //用来限制并发请求的数量。
            //该方法允许用户指定最大并发请求数，超过这个数量的请求将被拒绝或排队等待。
            Policy.Bulkhead(12);
            Policy.Bulkhead(12, 2);
            #endregion

        }

    }
}
