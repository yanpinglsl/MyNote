using GlobalizationLocalizationDemo.Extends;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;

namespace GlobalizationLocalizationDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews()
                .AddDataAnnotationsLocalization()//ע������ע�Ȿ�ػ�����            
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);//ͨ��LanguageViewLocationExpanderFormat.Suffixָ������ͼ��View��������Դ������ʽΪ��׺��
                                                                                //�� <view-name>.<language>.resx��

            //ͨ��AddLocalizationע����IStringLocalizerFactory��IStringLocalizer<>
            //��ָ������Դ�ĸ�Ŀ¼Ϊ��Resources��
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
            //�����������õ� json �ļ�
            //��Ҫ���� nuget �� My.Extensions.Localization.Json
            //builder.Services.AddJsonLocalization(options => options.ResourcesPath = "JsonResources");


            //ע��Swagger
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Swagger�ӿ��ĵ�",
                    Version = "v1",
                    Description = $"Core.WebApi HTTP API V1",
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                //c.SwaggerEndpoint("/swagger/v2/swagger.json", "�ӿ��ĵ�-����");//ҵ��ӿ��ĵ�������ʾ
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "�ӿ��ĵ�-ҵ��");//�����ӿ��ĵ��ź������ʾ
                //c.RoutePrefix = string.Empty;//���ú�ֱ������IP�Ϳ��Խ���ӿ��ĵ�
            });
            // �����м��
            app.UseRequestLocalization(options =>
            {
                var cultures = new[] { "zh-CN", "en-US", "zh-TW" };
                options.AddSupportedCultures(cultures);
                options.AddSupportedUICultures(cultures);
                options.SetDefaultCulture(cultures[0]);

                // ��Http��Ӧʱ���� ��ǰ������Ϣ ���õ� Response Header��Content-Language ��
                options.ApplyCurrentCultureToResponseHeaders = true;

                //ʵ���Զ��� RequestCultureProvider  
                options.RequestCultureProviders.Insert(0, new CustomHeaderRequestCultureProvider { HeaderName = "X-Lang" });
            });

            

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
