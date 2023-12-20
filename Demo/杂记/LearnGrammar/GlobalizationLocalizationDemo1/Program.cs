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

            //ͨ��AddLocalizationע����IStringLocalizerFactory��IStringLocalizer<>
            //��ָ������Դ�ĸ�Ŀ¼Ϊ��Resources��
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

            //builder.Services.AddMvc()
            //    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);
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
            });
            // Configure the HTTP request pipeline.
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
