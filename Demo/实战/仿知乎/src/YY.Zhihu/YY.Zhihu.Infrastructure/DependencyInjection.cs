using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YY.Zhihu.Domain.Data;
using YY.Zhihu.Infrastructure.Data.Repository;
using YY.Zhihu.Infrastructure.Identity;
using YY.Zhihu.Infrastructure.Interceptors;

namespace YY.Zhihu.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            ConfigureEfCore(services, configuration);

            ConfigureIdentity(services, configuration);

            return services;
        }
        private static IServiceCollection ConfigureEfCore(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddScoped<ISaveChangesInterceptor, AuditEntityInterceptor>();

            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });
            services.AddScoped<AppDbInitializer>();

            services.AddScoped(typeof(IReadRepository<>), typeof(EFReadRepository<>));
            services.AddScoped(typeof(IRepository<>), typeof(EFRepository<>));
            return services;
        }

        public static void ConfigureIdentity(this IServiceCollection services,
            IConfiguration configuration)
        {
            //services.AddScoped<IIdentityService, IdentityService>(); ;
            services.AddScoped<IdentityService>();

            services
                .AddIdentityCore<IdentityUser>(options =>
                {
                    options.User.RequireUniqueEmail = true;
                    options.Password.RequiredLength = 8;
                    options.Password.RequireDigit = true;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<AppDbContext>();

            // 从配置文件中读取JwtSettings，并注入到容器中
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        }
    }
}
