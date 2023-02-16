using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using WebCoreExtend.StartupExtend;

//[assembly: HostingStartup(typeof(CustomHostingStartupOut))]
namespace WebCoreExtend.StartupExtend
{
    public class CustomHostingStartupOut : IHostingStartup
    {
        public CustomHostingStartupOut()
        {
            Console.WriteLine($"********This is {nameof(CustomHostingStartupOut)} ctor********");
        }

        public void Configure(IWebHostBuilder builder)
        {
            Console.WriteLine($"********This is {nameof(CustomHostingStartupOut)} Configure********");

            #region 
            ////添加配置
            //builder.ConfigureAppConfiguration(configurationBuilder =>
            //{
            //    configurationBuilder.AddXmlFile("appsettings.xml", optional: false, reloadOnChange: true);
            //});

            ////IOC注册
            //builder.ConfigureServices(services =>
            //{
            //    services.AddTransient<ITestServiceA, TestServiceA>();
            //});

            builder.Configure(app =>
            {
                app.Use(next =>
                {
                    Console.WriteLine("This is CustomHostingStartupOut-Middleware  Init");
                    return new RequestDelegate(
                        async context =>
                        {
                            Console.WriteLine("This is CustomHostingStartupOut-Middleware start");
                            //await next.Invoke(context);//先不执行后面的
                            await Task.CompletedTask;
                            Console.WriteLine("This is CustomHostingStartupOut-Middleware end");
                        });
                });
            });//甚至来个中间件
            #endregion
        }
    }
}
