using YY.Zhihu.Domain.Data;
using YY.Zhihu.Infrastructure;
using YY.Zhihu.Infrastructure.Data.Repository;
using YY.Zhihu.UseCases;

namespace YY.Zhihu.Application
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddUseCaseServices();

            builder.Services.AddWebServices();

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                await app.InitializeDatabaseAsync();
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
