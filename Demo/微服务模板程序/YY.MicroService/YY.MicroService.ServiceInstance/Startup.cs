using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SkyApm.Utilities.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YY.MicroService.Framework;
using YY.MicroService.Framework.ConsulExtend;
using YY.MicroService.Framework.HttpApiExtend;
using YY.MicroService.Framework.ZipkinExtend;
using YY.MicroService.Interface;
using YY.MicroService.Service;
using zipkin4net;
using zipkin4net.Middleware;
using zipkin4net.Tracers.Zipkin;
using zipkin4net.Transport.Http;

namespace YY.MicroService.ServiceInstance
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();

            services.AddTransient<IUserService, UserService>();

            #region Consul Server IOC注册
            services.Configure<ConsulRegisterOptions>(this.Configuration.GetSection("ConsulRegisterOptions"));
            services.Configure<ConsulClientOptions>(this.Configuration.GetSection("ConsulClientOptions"));
            services.AddConsulRegister();
            services.AddConsulDispatcher(ConsulDispatcherType.Polling);
            #endregion

            #region SkyWalking
            //加skywalking监控链路
            services.AddSkyApmExtensions();
            #endregion

            #region Zipkin
            services.AddZipkin();
            #endregion

            services.AddHttpInvoker(options =>
            {
                options.Message = "This is Program's Message";
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "YY.MicroService.ServiceInstance", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "YY.MicroService.ServiceInstance v1"));
            }

            #region Consul注册
            app.UseHealthCheckMiddleware("/Api/Health/Index");//心跳请求响应
            app.ApplicationServices.GetService<IConsulRegister>()?.UseConsulRegist().Wait();
            #endregion

            #region 基于LifetTime对象注册Zipkin
            //IHostApplicationLifetime lifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>();
            //ILoggerFactory loggerFactory = app.ApplicationServices.GetService<ILoggerFactory>()!;
            //lifetime.ApplicationStarted.Register(() =>
            //{
            //    //记录数据密度，1.0代表全部记录
            //    TraceManager.SamplingRate = 1.0f;
            //    //链路日志
            //    var logger = new TracingLogger(loggerFactory, "zipkin4net");
            //    //zipkin服务地址和内容类型
            //    var httpSender = new HttpZipkinSender("http://192.168.200.104:9411/", "application/json");
            //    var tracer = new ZipkinTracer(httpSender, new JSONSpanSerializer(), new Statistics());
            //    var consoleTracer = new zipkin4net.Tracers.ConsoleTracer();

            //    TraceManager.RegisterTracer(tracer);
            //    TraceManager.RegisterTracer(consoleTracer);
            //    TraceManager.Start(logger);
            //});
            ////程序停止时停止链路跟踪
            //lifetime.ApplicationStopped.Register(() => TraceManager.Stop());
            ////引入zipkin中间件，用于跟踪服务请求,这边的名字可自定义代表当前服务名称
            //app.UseTracing("UserService");
            #endregion

            #region 使用扩展的Zipkin方式
            IHostApplicationLifetime lifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>();
            ILoggerFactory loggerFactory = app.ApplicationServices.GetService<ILoggerFactory>()!;
            app.UseZipkin(lifetime, loggerFactory!, "UserService", "http://192.168.200.104:9411/");
            #endregion


            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
