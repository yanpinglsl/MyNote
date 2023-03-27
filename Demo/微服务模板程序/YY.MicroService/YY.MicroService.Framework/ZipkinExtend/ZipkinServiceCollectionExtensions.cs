using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using zipkin4net;
using zipkin4net.Middleware;
using zipkin4net.Tracers.Zipkin;
using zipkin4net.Transport.Http;

namespace YY.MicroService.Framework.ZipkinExtend
{
    public static class ZipkinExtension
    {
        public static IServiceCollection AddZipkin(this IServiceCollection services)
        {
            services.AddSingleton<ITraceDiagnosticListener, HttpDiagnosticListener>();
            return services.AddSingleton<TraceObserver>();
        }

        public static IApplicationBuilder UseZipkin(this IApplicationBuilder app, IHostApplicationLifetime lifetime, ILoggerFactory loggerFactory, string serviceName, string zipkinUrl)
        {
            // 必须订阅
            DiagnosticListener.AllListeners.Subscribe(app.ApplicationServices.GetService<TraceObserver>()!);
            lifetime.ApplicationStarted.Register(() =>
            {
                //配置数据采样率，1.0代表全部采样
                TraceManager.SamplingRate = 1.0f;
                //链路日志
                var logger = new TracingLogger(loggerFactory, "zipkin4net");
                //zipkin服务地址和内容类型
                var httpSender = new HttpZipkinSender(zipkinUrl, "application/json");
                var tracer = new ZipkinTracer(httpSender, new JSONSpanSerializer(), new Statistics());
                var consoleTracer = new zipkin4net.Tracers.ConsoleTracer();

                TraceManager.RegisterTracer(tracer);
                TraceManager.RegisterTracer(consoleTracer);
                TraceManager.Start(logger);

            });
            //程序停止时停止链路跟踪
            lifetime.ApplicationStopped.Register(() => TraceManager.Stop());
            //引入zipkin中间件，用于跟踪服务请求,这边的名字可自定义代表当前服务名称
            app.UseTracing(serviceName);
            return app;
        }
    }
}
