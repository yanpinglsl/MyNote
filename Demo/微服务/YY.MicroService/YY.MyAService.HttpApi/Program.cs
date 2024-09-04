using YY.Common.Consul.ServiceRegistration;
using YY.Common.Consul.ServiceDiscovery;
using YY.MyAService.HttpApi.Clients;
using Consul.AspNetCore;
using Microsoft.AspNetCore.Builder;

namespace YY.MyAService.HttpApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            var serviceCheck = builder.Configuration
                .GetRequiredSection("ServiceCheck")
                .Get<ServiceCheckConfiguration>()!;

            builder.Services.AddConsul();
            builder.Services.AddConsulService(configuration =>
            {
                configuration.ServiceAddress = new Uri(builder.Configuration["urls"]!);
            }, serviceCheck);

            builder.Services.AddConsulClient();

            //builder.Services.AddHttpClient(nameof(MyBServiceClient), client =>
            //{
            //    client.Timeout = TimeSpan.FromSeconds(1);
            //});

            //builder.Services.AddSingleton<MyBServiceClient>();

            builder.Services.AddMyBServiceClient();

            builder.Services.AddHealthChecks();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            // Configure the HTTP request pipeline.

            app.UseHealthChecks(serviceCheck.Path);
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
