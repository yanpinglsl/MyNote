using Newtonsoft.Json;
using Polly;
using Polly.CircuitBreaker;
using Polly.Timeout;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using YY.MicroService.Model;

namespace Zhaoxi.MicroService.Polly
{
    class Program
    {
        static void Main(string[] args)
        {
            CircuitBreakPolicy();
            Console.ReadLine();
        }

        #region 超时策略
        public static async void TimeOutPolicy()
        {
            try
            {
                var memberJson = await Policy.TimeoutAsync(5, TimeoutStrategy.Pessimistic, (t, s, y) =>
                {
                    Console.WriteLine("超时了~~~~");
                    return Task.CompletedTask;
                }).ExecuteAsync(async () =>
                {
                    // 业务逻辑
                    using var httpClient = new HttpClient();
                    httpClient.BaseAddress = new Uri($"http://localhost:5000");
                    var memberResult = await httpClient.GetAsync("/api/polly/timeout");
                    memberResult.EnsureSuccessStatusCode();
                    var json = await memberResult.Content.ReadAsStringAsync();
                    Console.WriteLine(json);

                    return json;
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine("超时策略");

            }
        }
        #endregion

        #region Polly 重试策略
        public static async void RetryPolicy()
        {
            //当发生 HttpRequestException 的时候触发 RetryAsync 重试，并且最多重试3次。
            var memberJson1 = await Policy.Handle<HttpRequestException>().RetryAsync(3).ExecuteAsync(async () =>
            {
                Console.WriteLine("重试中.....");
                using var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri($"http://localhost:8000");
                var memberResult = await httpClient.GetAsync("/member/1001");
                memberResult.EnsureSuccessStatusCode();
                var json = await memberResult.Content.ReadAsStringAsync();

                return json;
            });

            //使用 Polly 在出现当请求结果为 http status_code 500 的时候进行3次重试。
            var memberResult = await Policy.HandleResult<HttpResponseMessage>
                (x => (int)x.StatusCode == 500).RetryAsync(3).ExecuteAsync(async () =>
            {
                Thread.Sleep(1000);
                Console.WriteLine("响应状态码重试中.....");
                using var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri($"http://localhost:5000");
                var memberResult = await httpClient.GetAsync("/api/polly/error");

                return memberResult;
            });

        }
        #endregion

        #region 服务降级
        public static async void LowerRankPolicy()
        {
            //首先我们使用 Policy 的 FallbackAsync("FALLBACK") 方法设置降级的返回值。当我们服务需要降级的时候会返回 "FALLBACK" 的固定值。
            //同时使用 WrapAsync 方法把重试策略包裹起来。这样我们就可以达到当服务调用失败的时候重试3次，如果重试依然失败那么返回值降级为固定的 "FALLBACK" 值。
            var fallback = Policy<string>.Handle<HttpRequestException>().Or<Exception>().FallbackAsync("FALLBACK", (x) =>
            {
                Console.WriteLine($"进行了服务降级 -- {x.Exception.Message}");
                return Task.CompletedTask;
            }).WrapAsync(Policy.Handle<HttpRequestException>().RetryAsync(3));

            var memberJson = await fallback.ExecuteAsync(async () =>
            {
                using var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri($"http://localhost:5000");
                var result = await httpClient.GetAsync("/api/user/" + 1);
                result.EnsureSuccessStatusCode();
                var json = await result.Content.ReadAsStringAsync();
                return json;

            });
            Console.WriteLine(memberJson);
            if (memberJson != "FALLBACK")
            {
                var member = JsonConvert.DeserializeObject<User>(memberJson);
                Console.WriteLine($"{member!.Id}---{member.Name}");
            }
        }
        #endregion

        #region 服务熔断
        public static async void CircuitBreakPolicy()
        {
            //定义熔断策略
            var circuitBreaker = Policy.Handle<Exception>().CircuitBreakerAsync(
               exceptionsAllowedBeforeBreaking: 2, // 出现几次异常就熔断
               durationOfBreak: TimeSpan.FromSeconds(10), // 熔断10秒
               onBreak: (ex, ts) =>
               {
                   Console.WriteLine("circuitBreaker onBreak ."); // 打开断路器
               },
               onReset: () =>
               {
                   Console.WriteLine("circuitBreaker onReset "); // 关闭断路器
               },
               onHalfOpen: () =>
               {
                   Console.WriteLine("circuitBreaker onHalfOpen"); // 半开
               }
            );

            // 定义重试策略
            var retry = Policy.Handle<HttpRequestException>().RetryAsync(3);
            // 定义降级策略
            var fallbackPolicy = Policy<string>.Handle<HttpRequestException>().Or<BrokenCircuitException>()
                .FallbackAsync("FALLBACK", (x) =>
                {
                    Console.WriteLine($"进行了服务降级 -- {x.Exception.Message}");
                    return Task.CompletedTask;
                })
                .WrapAsync(circuitBreaker.WrapAsync(retry));
            string memberJsonResult = "";

            do
            {
                memberJsonResult = await fallbackPolicy.ExecuteAsync(async () =>
                {
                    using var httpClient = new HttpClient();
                    httpClient.BaseAddress = new Uri($"http://localhost:5000");
                    var result = await httpClient.GetAsync("/api/user/" + 1);
                    result.EnsureSuccessStatusCode();
                    var json = await result.Content.ReadAsStringAsync();
                    return json;
                });
                Thread.Sleep(1000);
            } while (memberJsonResult == "FALLBACK");

            if (memberJsonResult != "FALLBACK")
            {
                var member = JsonConvert.DeserializeObject<User>(memberJsonResult);
                Console.WriteLine($"{member!.Id}---{member.Name}");
            }

        }
        #endregion
    }
}
