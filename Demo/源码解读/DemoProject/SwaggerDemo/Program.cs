using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace SwaggerDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            //需要引入Swashbuckle.AspNetCore
            builder.Services.AddSwaggerGen(options =>
            {
                #region 文档说明
                //作者、文档说明的信息
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "我的API-业务模块",
                    Description = "这是我的netcoreAPI项目",//描述信息
                    Contact = new OpenApiContact
                    {
                        Name = "我是YY",
                        Url = new Uri("https://baidu.com")
                    }
                });
                #endregion

                #region 接口注释
                //文档UI界面添加接口注释
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath, true);
                #endregion

                #region Swagger配置支持Token参数传递 
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "请输入token,格式为 Bearer jwtToken(注意中间必须有空格)",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });//添加安全定义

                options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {   //添加安全要求
                        new OpenApiSecurityScheme
                        {
                            Reference =new OpenApiReference()
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id ="Bearer"
                            }
                        },
                        new string[]{ }
                    }
                });
                #endregion

                #region 多版本
                foreach (FieldInfo field in typeof(ApiVersionInfo).GetFields())
                {
                    options.SwaggerDoc(field.Name, new Microsoft.OpenApi.Models.OpenApiInfo()
                    {
                        Title = $"API-{field.Name}",
                        Version = field.Name,
                        Description = $"当前的 ASP.Net Core Web API {field.Name} 版本"
                    });
                }
                #endregion

            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.

            //追加Swagger中间件
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                foreach (FieldInfo field in typeof(ApiVersionInfo).GetFields())
                {
                    c.SwaggerEndpoint($"/swagger/{field.Name}/swagger.json", $"{field.Name}");
                }
                //设置为None可折叠所有方法
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                //-1 可不显示Models
                c.DefaultModelsExpandDepth(-1);
            });

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
