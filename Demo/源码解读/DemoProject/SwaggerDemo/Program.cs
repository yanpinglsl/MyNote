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

            //��Ҫ����Swashbuckle.AspNetCore
            builder.Services.AddSwaggerGen(options =>
            {
                #region �ĵ�˵��
                //���ߡ��ĵ�˵������Ϣ
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "�ҵ�API-ҵ��ģ��",
                    Description = "�����ҵ�netcoreAPI��Ŀ",//������Ϣ
                    Contact = new OpenApiContact
                    {
                        Name = "����YY",
                        Url = new Uri("https://baidu.com")
                    }
                });
                #endregion

                #region �ӿ�ע��
                //�ĵ�UI������ӽӿ�ע��
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath, true);
                #endregion

                #region Swagger����֧��Token�������� 
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "������token,��ʽΪ Bearer jwtToken(ע���м�����пո�)",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });//��Ӱ�ȫ����

                options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {   //��Ӱ�ȫҪ��
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

                #region ��汾
                foreach (FieldInfo field in typeof(ApiVersionInfo).GetFields())
                {
                    options.SwaggerDoc(field.Name, new Microsoft.OpenApi.Models.OpenApiInfo()
                    {
                        Title = $"API-{field.Name}",
                        Version = field.Name,
                        Description = $"��ǰ�� ASP.Net Core Web API {field.Name} �汾"
                    });
                }
                #endregion

            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.

            //׷��Swagger�м��
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                foreach (FieldInfo field in typeof(ApiVersionInfo).GetFields())
                {
                    c.SwaggerEndpoint($"/swagger/{field.Name}/swagger.json", $"{field.Name}");
                }
                //����ΪNone���۵����з���
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                //-1 �ɲ���ʾModels
                c.DefaultModelsExpandDepth(-1);
            });

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
