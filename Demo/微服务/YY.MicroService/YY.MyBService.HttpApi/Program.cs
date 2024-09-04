using Consul.AspNetCore;
using Microsoft.AspNetCore.Builder;
using YY.Common.Consul.ServiceRegistration;

namespace YY.MyBService.HttpApi
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
