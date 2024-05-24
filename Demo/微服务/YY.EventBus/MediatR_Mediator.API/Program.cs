using System.Reflection;

namespace MediatR_Mediator.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddMediatR(configuration =>
            {
                //Handle所在程序集名称
                configuration.RegisterServicesFromAssembly(Assembly.Load("MediatR_Mediator.API"));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
