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
                    //Զ����֤��ַ
                    options.Authority = "https://localhost:5000";
                    //Httpsǿ��Ҫ���ʶ
                    options.RequireHttpsMetadata = true;

                    //�ͻ���ID
                    options.ClientId = "mvc";    //�ͻ���ID
                    options.ClientSecret = "123456"; //�ͻ�����Կ                                                 
                    options.ResponseType = OpenIdConnectResponseType.Code;    //��Ȩ��ģʽ
                    options.ResponseMode = OpenIdConnectResponseMode.Query;
                    options.SaveTokens = true;
                });
            // ����cookie����
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
