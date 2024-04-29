using Microsoft.AspNetCore.Builder;
using System.Reflection;

namespace CQRS.MediatR.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllers();
            builder.Services.AddMediatR(configuration =>
            {
                configuration.RegisterServicesFromAssembly(Assembly.Load("CQRS.MediatR.Core"));
            });

            //需要引入Swashbuckle.AspNetCore
            builder.Services.AddSwaggerGen();


            var app = builder.Build();

            // Configure the HTTP request pipeline.

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
