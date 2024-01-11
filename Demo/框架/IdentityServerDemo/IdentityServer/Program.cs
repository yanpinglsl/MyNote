using IdentityServer;

namespace IdentityServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();
            // Add services to the container.
            #region �ͻ���ģʽ��Ȩ
            ////ע��IdentityServer��������ע��һ�������ڴ�洢������ʱ״̬
            //builder.Services.AddIdentityServer(options =>
            //{
            //    //options.Events.RaiseErrorEvents = true;
            //    //options.Events.RaiseInformationEvents = true;
            //    //options.Events.RaiseFailureEvents = true;
            //    //options.Events.RaiseSuccessEvents = true;
            //})
            //    //����ģʽ�µ�ǩ��֤�飬�����������ü���
            //    .AddDeveloperSigningCredential()
            //    //OpenID Connect�����֤��Ϣ����
            //    //.AddInMemoryIdentityResources(Config.GetIdentityResources())
            //    //�����Դ����
            //    .AddInMemoryApiResources(Config.GetApis())//���ܱ�����Api��Դ��ӵ��ڴ���                                                         
            //    .AddInMemoryApiScopes(Config.GetApiScopes()) //���巶Χ
            //    .AddInMemoryClients(Config.GetClients());//�ͻ���������ӵ��ڴ�
            #endregion

            #region ��Դ������������Ȩģʽ
            builder.Services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApis())
                .AddInMemoryClients(Config.GetClients())
                .AddInMemoryApiScopes(Config.GetApiScopes())
                .AddTestUsers(Config.GetUsers());//�������û�ע�ᵽ IdentityServer��
            #endregion
            //ʹ��http����ʱ�ᱨ��cookie'.AspNetCore.Identity.Application'�����ˡ�SameSite=None�������������á�Secure
            builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
                options.OnAppendCookie = cookieContext =>
                    SetSameSite(cookieContext.Context, cookieContext.CookieOptions);
                options.OnDeleteCookie = cookieContext =>
                    SetSameSite(cookieContext.Context, cookieContext.CookieOptions);
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            //���IdentityServer�м��
            app.UseIdentityServer();

            app.UseRouting();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");


            app.Run();
        }
        public static void SetSameSite(HttpContext httpContext, CookieOptions options)
        {
            if (options.SameSite == SameSiteMode.None)
            {
                if (httpContext.Request.Scheme != "https")
                {
                    options.SameSite = SameSiteMode.Unspecified;
                }
            }
        }
    }
}
