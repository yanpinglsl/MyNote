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

            //需引入Microsoft.AspNetCore.Authentication.JwtBearer
            // 将身份认证服务添加到DI，并将“Bearer”配置为默认方案
            // AddJwtBearer 将 JWT 认证处理程序添加到DI中以供身份认证服务使用
            builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = "https://localhost:5001";//配置IdentityServer的授权地址
                    options.RequireHttpsMetadata = true;//不需要https
                    options.Audience = "group1";//api的name,需要与config的名称相同
                });
            //需引入Swashbuckle.AspNetCore包
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

            app.UseAuthentication();    // 将身份认证中间件添加到管道中，因此将在每次调用API时自动执行身份验证
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
