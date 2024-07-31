using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using StackExchange.Redis;
using System.Text;
using YY.Zhihu.Domain.Interfaces;
using YY.Zhihu.HotService.Jobs;
using YY.Zhihu.Infrastructure.Cache;
using YY.Zhihu.Infrastructure.Data;
using YY.Zhihu.Infrastructure.Data.Interceptors;
using YY.Zhihu.Infrastructure.Data.Repository;
using YY.Zhihu.Infrastructure.Identity;
using YY.Zhihu.Infrastructure.Interceptors;
using YY.Zhihu.Infrastructure.Quartz;
using YY.Zhihu.SharedLibraries.Repositoy;
using YY.Zhihu.UseCases.Contracts.Common.Interfaces;
using YY.Zhihu.UseCases.Contracts.Interfaces;
using YY.Zhihu.UseCases.Questions.Jobs;
using ZiggyCreatures.Caching.Fusion.Backplane.StackExchangeRedis;
using ZiggyCreatures.Caching.Fusion;
using Microsoft.Extensions.Caching.Distributed;

namespace YY.Zhihu.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            ConfigureEfCore(services, configuration);

            ConfigureIdentity(services, configuration);

            ConfigureQuartz(services, configuration);

            //ConfigureRedis(services, configuration);
            ConfigureCache(services, configuration);

            return services;
        }
        private static IServiceCollection ConfigureEfCore(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddScoped<ISaveChangesInterceptor, AuditEntityInterceptor>();
            services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });
            services.AddScoped<AppDbInitializer>();

            services.AddScoped(typeof(IReadRepository<>), typeof(EFReadRepository<>));
            services.AddScoped(typeof(IRepository<>), typeof(EFRepository<>));
            services.AddScoped<IAnswerRepository, AnswerRepository>();

            services.AddScoped<IDataQueryService, DataQueryService>();
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
            var configurationSection = configuration.GetSection("JwtSettings");
            var jwtSettings = configurationSection.Get<JwtSettings>();
            if (jwtSettings is null) 
                throw new NullReferenceException(nameof(jwtSettings));
            services.Configure<JwtSettings>(configurationSection);
            ConfigureAddAuthentication(services, jwtSettings);
        }
        public static void ConfigureAddAuthentication(this IServiceCollection services,
            JwtSettings jwtSettings)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                 .AddJwtBearer(options =>
                 {
                     options.TokenValidationParameters = new TokenValidationParameters
                     {
                         ClockSkew = TimeSpan.Zero,
                         ValidateIssuer = true,
                         ValidateAudience = true,
                         ValidateLifetime = true,
                         ValidateIssuerSigningKey = true,
                         ValidIssuer = jwtSettings.Issuer,
                         ValidAudience = jwtSettings.Audience,
                         IssuerSigningKey = new SymmetricSecurityKey(
                             Encoding.UTF8.GetBytes(jwtSettings.Secret)
                         )
                     };
                 });
        }
        //private static void ConfigureQuartz(IServiceCollection services, IConfiguration configuration)
        //{
        //    services.Configure<QuartzOptions>(configuration.GetSection("Quartz"));

        //    services.AddTransient<UpdateQuestionViewCountJob>();
        //    services.AddTransient<RefreshHotRankJob>();
        //    services.AddTransient<UpdateHotRankJob>();

        //    services.AddQuartz(configurator =>
        //    {
        //        configurator.CreateUpdateQuestionViewCountJobSchedule();
        //        configurator.CreateRefreshHotRankJobSchedule();
        //        configurator.CreateUpdateHotRankJobSchedule();
        //    });

        //    services.AddQuartzHostedService(opt =>
        //    {
        //        opt.WaitForJobsToComplete = true;
        //        opt.StartDelay = TimeSpan.FromSeconds(5);
        //    });
        //}

        private static void ConfigureQuartz(IServiceCollection services, IConfiguration configuration)
        {
            services.AddQuartzService(configuration);
        }

        //private static void ConfigureRedis(IServiceCollection services, IConfiguration configuration)
        //{
        //    var redisConn = configuration.GetConnectionString("RedisConnection");
        //    if (redisConn != null)
        //        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConn));
        //}

        private static void ConfigureCache(IServiceCollection services, IConfiguration configuration)
        {
            var redisConn = configuration.GetConnectionString("RedisConnection");
            if (redisConn != null)
                services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConn));

            services.AddStackExchangeRedisCache(options => options.Configuration = redisConn);
            services.AddFusionCache()
                .WithOptions(options =>
                {
                    options.DefaultEntryOptions = new FusionCacheEntryOptions { Duration = TimeSpan.FromMinutes(1) };
                })
                .WithSystemTextJsonSerializer()
                .WithDistributedCache(provider => provider.GetRequiredService<IDistributedCache>())
                .WithBackplane(new RedisBackplane(new RedisBackplaneOptions
                {
                    Configuration = redisConn
                }));

            services.AddSingleton(typeof(ICacheService<>), typeof(CacheService<>));
        }
    }
}
