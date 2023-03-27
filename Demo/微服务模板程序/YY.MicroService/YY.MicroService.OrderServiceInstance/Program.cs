using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YY.MicroService.Framework.PollyExtend;
using YY.MicroService.Interface;
using YY.MicroService.Service;

namespace YY.MicroService.OrderServiceInstance
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
        #region IOC容器
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureContainer<ContainerBuilder>((context, buider) =>
                {
                    // 必须使用单例注册
                    buider.RegisterType<UserService>()
                    .As<IUserService>().SingleInstance().EnableInterfaceInterceptors();
                    buider.RegisterType<PollyPolicyAttribute>();

                });
        #endregion
    }
}
