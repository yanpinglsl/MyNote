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
                .AddDataAnnotationsLocalization()//注册数据注解本地化服务            
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);//通过LanguageViewLocationExpanderFormat.Suffix指定了视图（View）语言资源命名格式为后缀，
                                                                                //即 <view-name>.<language>.resx。

            //通过AddLocalization注册了IStringLocalizerFactory和IStringLocalizer<>
            //并指定了资源的根目录为“Resources”
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
            //将多语言配置到 json 文件
            //需要引用 nuget 包 My.Extensions.Localization.Json
            //builder.Services.AddJsonLocalization(options => options.ResourcesPath = "JsonResources");


            //注册Swagger
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Swagger接口文档",
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
                //c.SwaggerEndpoint("/swagger/v2/swagger.json", "接口文档-基础");//业务接口文档首先显示
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "接口文档-业务");//基础接口文档放后面后显示
                //c.RoutePrefix = string.Empty;//设置后直接输入IP就可以进入接口文档
            });
            // 启用中间件
            app.UseRequestLocalization(options =>
            {
                var cultures = new[] { "zh-CN", "en-US", "zh-TW" };
                options.AddSupportedCultures(cultures);
                options.AddSupportedUICultures(cultures);
                options.SetDefaultCulture(cultures[0]);

                // 当Http响应时，将 当前区域信息 设置到 Response Header：Content-Language 中
                options.ApplyCurrentCultureToResponseHeaders = true;

                //实现自定义 RequestCultureProvider  
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
