using Microsoft.EntityFrameworkCore;
using YY.Docker.Http.API;

namespace YY.Docker.Http.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            Console.WriteLine(builder.Configuration.GetConnectionString("DefaultConnection"));

            builder.Services.AddDbContext<ApplicationDbContext>(optionsBuilder =>
            {
                //optionsBuilder.UseMySql(
                //    builder.Configuration.GetConnectionString("DefaultConnection"),
                //    new MySqlServerVersion(new Version(5, 7)));
                optionsBuilder.UseMySql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    new MySqlServerVersion(new Version(5, 7)));
            });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.Run();

        }
    }
}
