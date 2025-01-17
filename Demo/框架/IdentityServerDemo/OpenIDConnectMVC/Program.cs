using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.IdentityModel.Tokens.Jwt;

namespace OpenIDConnectMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();


            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

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
                    #region OpenIdConnect简化流程
                    //远程认证地址
                    //与 RequireHttpsMetadata属性要同步
                    options.Authority = "https://localhost:5001";
                    //Https强制要求标识
                    options.RequireHttpsMetadata = true;

                    //客户端ID
                    options.ClientId = "mvc";    //客户端ID
                    options.ClientSecret = "secret"; //客户端秘钥                                                 
                    options.ResponseType = OpenIdConnectResponseType.IdTokenToken;
                    //options.ResponseMode = OpenIdConnectResponseMode.Query;
                    options.SaveTokens = true;
                    #endregion

                    #region 混合模式
                    options.SignInScheme = "Cookies";

                    options.Authority = "https://localhost:5001";
                    options.RequireHttpsMetadata = true;

                    options.ClientId = "hybrid";
                    options.ClientSecret = "secret";
                    options.ResponseType = OpenIdConnectResponseType.CodeIdToken;

                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;

                    options.Scope.Add("group1");
                    options.Scope.Add("offline_access");
                    options.ClaimActions.MapJsonKey("website", "website");
                    #endregion
                });
            // 配置cookie策略
            //services.AddNonBreakingSameSiteCookies();

            var app = builder.Build();

            // Configure the HTTP request pipeline.     
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
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
