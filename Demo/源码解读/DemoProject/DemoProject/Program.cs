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
            #region ����WebApplicationBuilder
            var builder = WebApplication.CreateBuilder(args);
            #endregion

            #region ����WebApplicationBuilder

            #region Configuration
            //Ĭ�Ͼ���appsettings.json---commandline
            builder.Configuration.AddJsonFile("customjson.json", true, true);
            builder.Configuration.AddXmlFile("customxml.xml", true, true);
            builder.Configuration.AddIniFile("customini.ini", true, true);
            //builder.Configuration.AddCommandLine();
            //��֧�ָ�������Դ

            #region �Զ���ģʽ

            ////((IConfigurationBuilder)builder.Configuration).Add(new CustomConfigurationSource());

            ////builder.Configuration.Add(new CustomConfigurationSource());
            ////A�� ʵ����B�ӿڣ�B�ӿ�����Add����   
            ////������A��ʵ��ֱ��ȥ����Add��������
            ////�������Aǿ��ת����B�ӿ����ͣ��Ͳ�������
            ////��Ϊ�ӿڵ�Add��������ʽʵ�֣���ô����ʱ�ͱ���ת����B�ӿ�����---IConfigurationBuilder.Add
                        
            builder.Configuration.AddCustomConfiguration();
            #endregion

            #region �ڴ�Provider
            var memoryConfig = new Dictionary<string, string>
                {
                   {"TodayMemory", "Memory����"},
                   {"MemoryOptions:HostName", "192.168.200.104"},
                   {"MemoryOptions:UserName", "Memory"},
                   {"MemoryOptions:Password", "Memory8888"}
                };
            builder.Configuration.AddInMemoryCollection(memoryConfig);
            #endregion

            #endregion

            #region ��־�����չ
            //builder.Logging.ClearProviders();
            //builder.Logging.AddConsole()
            //    .AddDebug();
            //builder.Logging.AddLog4Net();//Microsoft.Extensions.Logging.Log4Net.AspNetCore

            //builder.Logging.AddFilter("System", LogLevel.Warning);//System��ͷ��warning�����ϼ���
            //builder.Logging.AddFilter("Microsoft", LogLevel.Warning);

            #region �Զ�����־���
            ////builder.Logging.AddProvider(new CustomConsoleLoggerProvider());//������ģ�����׼
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
            //��Ĭ������
            builder.Services.Configure<EmailOptions>(op =>
            {
                op.Title = "Title--DefaultName";
                op.From = "From---DefaultName";
            });
            //��ָ������
            builder.Services.Configure<EmailOptions>("FromMemory", op =>
            {
                op.Title = "Title---FromMemory";
                op.From = "From---FromMemory";
            });
            //�۴������ļ���ȡ
            builder.Services.Configure<EmailOptions>("FromConfiguration", builder.Configuration.GetSection("Email"));
            //�ܵȼ��ڢ�
            builder.Services.AddOptions<EmailOptions>("AddOption").Configure(op => 
            {
                op.Title = "Title---AddOption";
                op.From = "From---AddOption";
            });
            ////�ݵ�������Ϊnullʱ�����������EmailOptions����
            //builder.Services.Configure<EmailOptions>(null, op =>
            //{
            //    op.Title = "Title---Configure--null";
            //    op.From = "From---Configure--null";
            //});
            ////�޵ȼ��ڢ�
            //builder.Services.ConfigureAll<EmailOptions>(op => op.From = "ConfigureAll");

            ////�ߵȼ��ڢݣ�PostConfiger����Configer�����ϼ�������
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