using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using YY.Zhihu.Domain;
using YY.Zhihu.Domain.Interfaces;
using YY.Zhihu.Infrastructure;
using YY.Zhihu.Infrastructure.Data;
using YY.Zhihu.Infrastructure.Data.Interceptors;
using YY.Zhihu.Infrastructure.Data.Repository;
using YY.Zhihu.Infrastructure.Interceptors;
using YY.Zhihu.SharedLibraries.Repositoy;
using YY.Zhihu.UseCases.Contracts.Interfaces;

namespace YY.Zhihu.UseCases.Tests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var mockUser = new Mock<IUser>();
            mockUser.Setup(user => user.Id).Returns(1);
            services.AddSingleton(mockUser.Object);

            ConfigureEfCore(services);

            services.AddScoped<DbInitializer>();

            services.AddUseCaseServices();
            services.AddCoreServices();
        }

        private static void ConfigureEfCore(IServiceCollection services)
        {
            services.AddScoped<ISaveChangesInterceptor, AuditEntityInterceptor>();
            services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

                options.UseSqlite("Data Source=:memory:");
            });

            services.AddScoped(typeof(IReadRepository<>), typeof(EFReadRepository<>));
            services.AddScoped(typeof(IRepository<>), typeof(EFRepository<>));
            services.AddScoped<IAnswerRepository, AnswerRepository>();

            services.AddScoped<IDataQueryService, DataQueryService>();
        }
    }
}

