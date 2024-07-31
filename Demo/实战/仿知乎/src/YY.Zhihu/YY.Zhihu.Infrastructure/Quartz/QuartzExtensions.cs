using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz.Impl;
using Quartz.Logging;
using Quartz.Simpl;
using Quartz.Spi;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection.Extensions;
using YY.Zhihu.HotService.Jobs;
using YY.Zhihu.UseCases.Questions.Jobs;

namespace YY.Zhihu.Infrastructure.Quartz
{
    public static class QuartzExtensions
    {
        public static void AddQuartzService(this IServiceCollection services, IConfiguration configuration)
        {
            LogProvider.SetCurrentLogProvider(new ConsoleLogProvider());
            var quartzOption = configuration.GetSection("Quartz").Get<QuartzOption>();

            if (quartzOption == null) return;

            foreach (var schedulerOption in quartzOption.Schedulers)
            {
                var scheduler = SchedulerBuilder
                    .Create(schedulerOption.ToNameValueCollection())
                    .BuildScheduler().Result;

                services.AddSingleton(scheduler);
            }

            services.TryAddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.TryAddSingleton<IJobFactory, MicrosoftDependencyInjectionJobFactory>();

            services.AddHostedService<QuartzHostedService>();
        }

        public static void AddJobSchedule(this IScheduler scheduler)
        {
            switch (scheduler.SchedulerName)
            {
                case "YY.ZhiHu.ClusteredScheduler":
                    scheduler.CreateRefreshHotRankJobSchedule();
                    scheduler.CreateUpdateHotRankJobSchedule();
                    break;
                case "YY.ZhiHu.LocalScheduler":
                    scheduler.CreateUpdateQuestionViewCountJobSchedule();
                    break;
            }

        }
    }
}
