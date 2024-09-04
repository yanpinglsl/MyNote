using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using Polly.Fallback;
using Polly.Timeout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace YY.Common.Polly
{
    public static class ResiliencePipelineExtension
    {
        public static IHttpClientBuilder AddCustomResiliencePipeline(this IHttpClientBuilder builder)
        {
            builder.AddResilienceHandler("common", pipelineBuilder =>
            {
                pipelineBuilder.AddFallback(new FallbackStrategyOptions<HttpResponseMessage>
                {
                    FallbackAction = _ =>
                    {
                        Console.WriteLine("降级操作");
                        return Outcome.FromResultAsValueTask(new HttpResponseMessage
                        {
                            Content = new StringContent("Hello 我是降级操作"),
                            StatusCode = HttpStatusCode.OK,
                        });
                    }
                });

                pipelineBuilder.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
                {
                    FailureRatio = 0.5,
                    SamplingDuration = TimeSpan.FromSeconds(60),
                    MinimumThroughput = 2,
                    BreakDuration = TimeSpan.FromSeconds(60),
                    OnOpened = _ =>
                    {
                        Console.WriteLine($"{DateTime.Now} - 断路器：开启状态（熔断时触发）");
                        return default;
                    },
                    OnClosed = _ =>
                    {
                        Console.WriteLine($"{DateTime.Now} - 断路器：关闭状态（恢复时触发）");
                        return default;
                    },
                    OnHalfOpened = _ =>
                    {
                        Console.WriteLine($"{DateTime.Now} - 断路器：半开启状态");
                        return default;
                    }
                });

                pipelineBuilder.AddRetry(new HttpRetryStrategyOptions
                {
                    MaxRetryAttempts = 2,
                    Delay = TimeSpan.FromSeconds(1),
                    OnRetry = arg =>
                    {
                        Console.WriteLine($"{DateTime.Now} - 重试 {arg.AttemptNumber} 次");
                        return default;
                    }
                });

                pipelineBuilder.AddTimeout(new TimeoutStrategyOptions
                {
                    Timeout = TimeSpan.FromSeconds(1),
                    OnTimeout = arg =>
                    {
                        Console.WriteLine($"{DateTime.Now} - 请求超时");
                        return default;
                    }
                });
            });
            return builder;
        }
    }
}
