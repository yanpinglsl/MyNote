using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.IdentityModel.Tokens.Jwt;

namespace OpenIDConnectDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";//设置验证时使用的默认方案
                options.DefaultChallengeScheme = "oidc";//默认方案验证失败后的确认验证结果方案
                                                        //options.DefaultForbidScheme = "Cookie";//设置禁止访问时使用的默认方案
                                                        //options.DefaultSignInScheme = "Cookie"; //设置登录的默认方案。
                                                        //options.DefaultSignOutScheme = "Cookie";//设置退出的默认方案。
            })
                .AddCookie("Cookies")
                //添加OpenIdConnect认证方案
                //需要nuget Microsoft.AspNetCore.Authentication.OpenIdConnect
                .AddOpenIdConnect("oidc", options =>
                {
                    //远程认证地址
                    options.Authority = "https://localhost:5000";
                    //Https强制要求标识
                    options.RequireHttpsMetadata = true;

                    //客户端ID
                    options.ClientId = "mvc";    //客户端ID
                    options.ClientSecret = "123456"; //客户端秘钥                                                 
                    options.ResponseType = OpenIdConnectResponseType.Code;    //授权码模式
                    options.ResponseMode = OpenIdConnectResponseMode.Query;
                    options.SaveTokens = true;
                });
            // 配置cookie策略
            //services.AddNonBreakingSameSiteCookies();

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

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
