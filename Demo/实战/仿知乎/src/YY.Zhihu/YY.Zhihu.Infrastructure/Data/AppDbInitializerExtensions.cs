using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace YY.Zhihu.Infrastructure
{
    public static class AppDbInitializerExtensions
    {
        public static async Task InitializeDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var initializer = scope.ServiceProvider.GetRequiredService<AppDbInitializer>();

            await initializer.InitializeAsync();

            await initializer.SeedAsync();
        }
    }
}
