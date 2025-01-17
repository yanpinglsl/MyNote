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
            #region 客户端模式授权
            ////注册IdentityServer，它还会注册一个基于内存存储的运行时状态
            //builder.Services.AddIdentityServer(options =>
            //{
            //    //options.Events.RaiseErrorEvents = true;
            //    //options.Events.RaiseInformationEvents = true;
            //    //options.Events.RaiseFailureEvents = true;
            //    //options.Events.RaiseSuccessEvents = true;
            //})
            //    //开发模式下的签名证书，开发环境启用即可
            //    .AddDeveloperSigningCredential()
            //    //OpenID Connect相关认证信息配置
            //    //.AddInMemoryIdentityResources(Config.GetIdentityResources())
            //    //相关资源配置
            //    .AddInMemoryApiResources(Config.GetApis())//把受保护的Api资源添加到内存中                                                         
            //    .AddInMemoryApiScopes(Config.GetApiScopes()) //定义范围
            //    .AddInMemoryClients(Config.GetClients());//客户端配置添加到内存
            #endregion

            #region 资源所有者密码授权模式
            builder.Services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApis())
                .AddInMemoryClients(Config.GetClients())
                .AddInMemoryApiScopes(Config.GetApiScopes())
                .AddTestUsers(Config.GetUsers());//将测试用户注册到 IdentityServer：
            #endregion
            //使用http访问时会报错：cookie'.AspNetCore.Identity.Application'设置了“SameSite=None”，还必须设置“Secure
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

            //添加IdentityServer中间件
            app.UseIdentityServer();

            app.UseRouting();
            app.UseCookiePolicy();
            //app.UseAuthentication();
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
