using Microsoft.OpenApi.Models;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddAuthorization();

            //������Microsoft.AspNetCore.Authentication.JwtBearer
            // �������֤������ӵ�DI��������Bearer������ΪĬ�Ϸ���
            // AddJwtBearer �� JWT ��֤���������ӵ�DI���Թ������֤����ʹ��
            builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = "http://localhost:5000";//����IdentityServer����Ȩ��ַ
                    options.RequireHttpsMetadata = false;//����Ҫhttps
                    options.Audience = "group1";//api��name,��Ҫ��config��������ͬ
                });
            //������Swashbuckle.AspNetCore��
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Api", Version = "v1" });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "IEMS v1"));
            }

            //app.UseHttpsRedirection();

            app.UseAuthentication();    // �������֤�м����ӵ��ܵ��У���˽���ÿ�ε���APIʱ�Զ�ִ�������֤
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
