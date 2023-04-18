
using Microsoft.OpenApi.Models;
using System.Security.Cryptography;
using WebCoreExtend.JWTExtend.RSA;
using WebCoreExtend.JWTExtend;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AuthenticationCenter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();


            #region Swagger配置
            builder.Services.AddSwaggerGen(options =>
            {
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
            });
            #endregion



            #region HS256 对称可逆加密
            builder.Services.AddScoped<IJWTService, JWTHSService>();
            builder.Services.Configure<JWTTokenOptions>(builder.Configuration.GetSection("JWTTokenOptions"));
            #endregion

            #region RS256 非对称可逆加密，需要获取一次公钥
            ////程序启动时，即初始化一组秘钥
            //string keyDir = Directory.GetCurrentDirectory();
            //if (!RSAHelper.TryGetKeyParameters(keyDir, true, out RSAParameters keyParams))
            //{
            //    keyParams = RSAHelper.GenerateAndSaveKey(keyDir);
            //}

            //builder.Services.AddScoped<IJWTService, JWTRSService>();
            //builder.Services.Configure<JWTTokenOptions>(builder.Configuration.GetSection("JWTTokenOptions"));
            #endregion


            #region 鉴权授权
            JWTTokenOptions tokenOptions = new JWTTokenOptions();
            builder.Configuration.Bind("JWTTokenOptions", tokenOptions);

            builder.Services.AddAuthentication
                (JwtBearerDefaults.AuthenticationScheme)//用JWT方式鉴权
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,//是否验证Issuer
                        ValidateAudience = true,//是否验证Audience
                        ValidateLifetime = false,//是否验证失效时间
                        ValidateIssuerSigningKey = true,//是否验证SecurityKey
                        ValidAudience = tokenOptions.Audience,//
                        ValidIssuer = tokenOptions.Issuer,//Issuer，这两项和前面签发jwt的设置一致
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecurityKey))
                    };
                });
            #endregion


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}