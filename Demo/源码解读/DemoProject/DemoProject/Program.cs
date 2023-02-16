using DemoProject.Models;
using DemoProject.Utility;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Extensions.Configuration;
using System.Drawing;
using WebCoreExtend.ConfigurationExtend;
using WebCoreExtend.LogExtend;

namespace DemoProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #region 创建WebApplicationBuilder
            var builder = WebApplication.CreateBuilder(args);
            #endregion

            #region 配置WebApplicationBuilder

            #region Configuration
            //默认就有appsettings.json---commandline
            builder.Configuration.AddJsonFile("customjson.json", true, true);
            builder.Configuration.AddXmlFile("customxml.xml", true, true);
            builder.Configuration.AddIniFile("customini.ini", true, true);
            //builder.Configuration.AddCommandLine();
            //还支持各种数据源

            #region 自定义模式

            ////((IConfigurationBuilder)builder.Configuration).Add(new CustomConfigurationSource());

            ////builder.Configuration.Add(new CustomConfigurationSource());
            ////A类 实现了B接口，B接口是有Add方法   
            ////但是用A类实例直接去调用Add方法报错
            ////但如果把A强制转换成B接口类型，就不报错了
            ////因为接口的Add方法是显式实现，那么调用时就必须转换成B接口类型---IConfigurationBuilder.Add
                        
            builder.Configuration.AddCustomConfiguration();
            #endregion

            #region 内存Provider
            var memoryConfig = new Dictionary<string, string>
                {
                   {"TodayMemory", "Memory配置"},
                   {"MemoryOptions:HostName", "192.168.200.104"},
                   {"MemoryOptions:UserName", "Memory"},
                   {"MemoryOptions:Password", "Memory8888"}
                };
            builder.Configuration.AddInMemoryCollection(memoryConfig);
            #endregion

            #endregion

            #region 日志组件扩展
            //builder.Logging.ClearProviders();
            //builder.Logging.AddConsole()
            //    .AddDebug();
            //builder.Logging.AddLog4Net();//Microsoft.Extensions.Logging.Log4Net.AspNetCore

            //builder.Logging.AddFilter("System", LogLevel.Warning);//System开头，warning及以上级别
            //builder.Logging.AddFilter("Microsoft", LogLevel.Warning);

            #region 自定义日志组件
            ////builder.Logging.AddProvider(new CustomConsoleLoggerProvider());//最初级的，不标准
            ////builder.Logging.AddProvider(new CustomConsoleLoggerProvider(new CustomConsoleLoggerOptions()
            ////{
            ////    MinLogLevel = LogLevel.Warning,
            ////    ConsoleColor = ConsoleColor.Green,
            ////}));

            ////builder.Logging.AddCustomLogger();
            ////builder.Logging.AddCustomLogger(new CustomConsoleLoggerOptions()
            ////{
            ////    MinLogLevel = LogLevel.Warning,
            ////    ConsoleColor = ConsoleColor.Green,
            ////});
            //builder.Logging.AddCustomLogger(options =>
            //{
            //    options.MinLogLevel = LogLevel.Debug;
            //    options.ConsoleColor = ConsoleColor.Yellow;

            //});

            #endregion

            #endregion


            #endregion


            // Add services to the container.
            builder.Services.AddControllersWithViews();


            #region Options
            //①默认名称
            builder.Services.Configure<EmailOptions>(op =>
            {
                op.Title = "Title--DefaultName";
                op.From = "From---DefaultName";
            });
            //②指定名称
            builder.Services.Configure<EmailOptions>("FromMemory", op =>
            {
                op.Title = "Title---FromMemory";
                op.From = "From---FromMemory";
            });
            //③从配置文件读取
            builder.Services.Configure<EmailOptions>("FromConfiguration", builder.Configuration.GetSection("Email"));
            //④等价于②
            builder.Services.AddOptions<EmailOptions>("AddOption").Configure(op => 
            {
                op.Title = "Title---AddOption";
                op.From = "From---AddOption";
            });
            ////⑤当配置名为null时，会更新所有EmailOptions配置
            //builder.Services.Configure<EmailOptions>(null, op =>
            //{
            //    op.Title = "Title---Configure--null";
            //    op.From = "From---Configure--null";
            //});
            ////⑥等价于⑤
            //builder.Services.ConfigureAll<EmailOptions>(op => op.From = "ConfigureAll");

            ////⑦等价于⑤，PostConfiger可在Configer基础上继续配置
            //builder.Services.PostConfigure<EmailOptions>(null, op =>
            //{
            //    op.Title = "Title---PostConfigure--null";
            //    op.From = "From---PostConfigure--null";
            //});
            //builder.Services.PostConfigureAll<EmailOptions>(op => op.Body = "services.PostConfigure<EmailOption>--Name null--Same With PostConfigureAll");
            #endregion

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}