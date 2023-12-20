using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Globalization;

namespace GlobalizationLocalizationDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            //通过AddLocalization注册了IStringLocalizerFactory和IStringLocalizer<>
            //并指定了资源的根目录为“Resources”
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

            //builder.Services.AddMvc()
            //    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);
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
            });
            // Configure the HTTP request pipeline.
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
