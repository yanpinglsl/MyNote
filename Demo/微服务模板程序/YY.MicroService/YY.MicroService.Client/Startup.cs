using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YY.MicroService.Framework.ConsulExtend;
using YY.MicroService.Framework.HttpApiExtend;
using YY.MicroService.gRPCService;
using YY.MicroService.Interface;
using YY.MicroService.Service;

namespace YY.MicroService.Client
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IHttpAPIInvoker, HttpAPIInvoker>();
            #region gRPC Client注册
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            //需要引入Nuget包：Grpc.Net.ClientFactory
            services.AddGrpcClient<MyTest.MyTestClient>(options =>
            {
                options.Address = new Uri("http://localhost:5017");
                //options.Address = new Uri("https://localhost:7017");
            }).ConfigureChannel(grpcOptions =>
            {
                //可以完成各种配置  比如证书   比如token
            });
            #endregion
            #region Consul Client IOC注册
            services.Configure<ConsulClientOptions>(this.Configuration.GetSection("ConsulClientOptions"));
            services.AddConsulDispatcher(ConsulDispatcherType.Polling);
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
