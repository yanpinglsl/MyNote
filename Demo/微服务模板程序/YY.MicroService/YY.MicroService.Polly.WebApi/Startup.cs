using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YY.MicroService.Model;

namespace Zhaoxi.MicroService.Polly.WebApi
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

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Zhaoxi.MicroService.Polly.WebApi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Zhaoxi.MicroService.Polly.WebApi v1"));
            }

            app.Map("/api/polly/timeout", app =>
            {
                app.Run(context =>
                {
                    Thread.Sleep(6000);
                    return context.Response.WriteAsync("Polly Timeout");
                });
            });

            app.Map("/api/polly/error", app =>
            {
                app.Run(context =>
                {
                    context.Response.StatusCode = 500;
                    return context.Response.WriteAsync("Polly Fail");
                });
            });

            app.Map("/api/user/1", app =>
            {
                app.Run(context =>
                {
                    var user = new User
                    {
                        Id = 20001,
                        Name = "Zhaoxi Gerry",
                    };

                    return context.Response.WriteAsync(JsonConvert.SerializeObject(user));
                });
            });
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
