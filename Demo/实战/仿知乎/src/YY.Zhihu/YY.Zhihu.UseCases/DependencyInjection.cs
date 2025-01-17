﻿using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using YY.Zhihu.HotService;
using YY.Zhihu.UseCases.Common.Behaviors;
using YY.Zhihu.UseCases.Questions.Jobs;

namespace YY.Zhihu.UseCases
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUseCaseServices(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.RegisterServicesFromAssembly(Assembly.GetAssembly(typeof(Domain.DependencyInjection))!);
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            });
            services.AddSingleton<QuestionViewCountService>();
            services.AddHotService();
            return services;
        }
    }
}