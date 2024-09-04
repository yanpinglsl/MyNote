using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YY.Polly.BasicUse
{
    public class PolicyFactory
    {
        /// <summary>
        /// 多策略组合包装
        /// 场景：服务A（集群、多个实例）=》请求发生异常或者超时=》重新再次请求=》重试3次仍然失败=》切断对A的访问，响应1个替代结果，过一段时间尝试访问A=》是否恢复正常
        /// </summary>
        /// <returns></returns>
        public static ISyncPolicy CreatePolicy()
        {
            // 定义故障
            var builder = Policy.Handle<Exception>();

            // 等待并重试
            var retryPolicy = builder.WaitAndRetry(2,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (_, _, retryCount, _) =>
                {
                    Console.WriteLine($"{DateTime.Now} - 重试 {retryCount} 次");
                });

            // 断路器
            var circuitPolicy = builder.CircuitBreaker(2, TimeSpan.FromSeconds(5),
                onBreak: (_, _) =>
                {
                    Console.WriteLine($"{DateTime.Now} - 断路器：开启状态（熔断时触发）");
                },
                onReset: () =>
                {
                    Console.WriteLine($"{DateTime.Now} - 断路器：关闭状态（恢复时触发）");
                },
                onHalfOpen: () =>
                {
                    Console.WriteLine($"{DateTime.Now} - 断路器：半开启状态");
                });

            // 降级回退
            var fallbackPolicy = builder.Fallback(() =>
            {
                Console.WriteLine("这是一个降级操作");
            });

            // 超时
            var timeoutPolicy = Policy.Timeout(1, (_, _, _) =>
            {
                Console.WriteLine("执行超时");
            });

            // 策略包装：从左到右
            return Policy.Wrap(fallbackPolicy, circuitPolicy, retryPolicy, timeoutPolicy);

        }

    }
}
