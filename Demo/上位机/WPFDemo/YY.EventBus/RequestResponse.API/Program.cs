using System.Reflection;

namespace RequestResponse.API
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
                //Handle���ڳ�������
                configuration.RegisterServicesFromAssembly(Assembly.Load("RequestResponse.API"));
            });
            //��Ҫ����Swashbuckle.AspNetCore
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
