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
                options.DefaultScheme = "Cookies";//������֤ʱʹ�õ�Ĭ�Ϸ���
                options.DefaultChallengeScheme = "oidc";//Ĭ�Ϸ�����֤ʧ�ܺ��ȷ����֤�������
                                                        //options.DefaultForbidScheme = "Cookie";//���ý�ֹ����ʱʹ�õ�Ĭ�Ϸ���
                                                        //options.DefaultSignInScheme = "Cookie"; //���õ�¼��Ĭ�Ϸ�����
                                                        //options.DefaultSignOutScheme = "Cookie";//�����˳���Ĭ�Ϸ�����
            })
                .AddCookie("Cookies")
                //���OpenIdConnect��֤����
                //��Ҫnuget Microsoft.AspNetCore.Authentication.OpenIdConnect
                .AddOpenIdConnect("oidc", options =>
                {
                    #region OpenIdConnect������
                    //Զ����֤��ַ
                    //�� RequireHttpsMetadata����Ҫͬ��
                    options.Authority = "https://localhost:5001";
                    //Httpsǿ��Ҫ���ʶ
                    options.RequireHttpsMetadata = true;

                    //�ͻ���ID
                    options.ClientId = "mvc";    //�ͻ���ID
                    options.ClientSecret = "secret"; //�ͻ�����Կ                                                 
                    options.ResponseType = OpenIdConnectResponseType.IdTokenToken;
                    //options.ResponseMode = OpenIdConnectResponseMode.Query;
                    options.SaveTokens = true;
                    #endregion

                    #region ���ģʽ
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
            // ����cookie����
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
