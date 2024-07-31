using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.Domain.QuestionAggerate.Entites;

namespace YY.Zhihu.HotService.Jobs
{
    public static class UpdateHotRankSchedule
    {

        //使用Quartz提供的依赖注入只支持单调度器
        //NuGet：Quartz.Extensions.DependencyInjection
        //public static void CreateUpdateHotRankJobSchedule(
        //    this IServiceCollectionQuartzConfigurator configurator,
        //    int intervalTime = 60)
        //{
        //    configurator.AddJob<UpdateHotRankJob>(triggerConfigurator => triggerConfigurator
        //        .WithIdentity(UpdateHotRankJob.Key));

        //    configurator.AddTrigger(trigger => trigger
        //        .WithIdentity(Key)
        //        .ForJob(UpdateHotRankJob.Key)
        //        .StartAt(DateBuilder.FutureDate(intervalTime, IntervalUnit.Second))
        //        .WithSimpleSchedule(builder => builder
        //            .RepeatForever()
        //            .WithInterval(TimeSpan.FromSeconds(intervalTime))
        //        ));
        //}

        //使用Quartz组件自定义多调度器
        //NuGet：Quartz.Jobs
        public static void CreateUpdateHotRankJobSchedule(
            this IScheduler scheduler,
            int intervalTime = 60)
        {
            var schedulerId = scheduler.SchedulerInstanceId;
            var triggerKey = new TriggerKey($"{nameof(UpdateHotRankSchedule)}-{schedulerId}", nameof(UpdateHotRankJob));
            var jobDetail = JobBuilder.Create<UpdateHotRankJob>()
                .WithIdentity(UpdateHotRankJob.Key)
                .Build();
            var triggers = new List<ITrigger>()
            {
                TriggerBuilder.Create()
                .WithIdentity(triggerKey)
                .ForJob(UpdateHotRankJob.Key)
                .StartAt(DateBuilder.FutureDate(intervalTime, IntervalUnit.Second))
                .WithSimpleSchedule(builder => builder
                    .RepeatForever()
                    .WithInterval(TimeSpan.FromSeconds(intervalTime)))
                 .Build()
            };

            scheduler.ScheduleJob(jobDetail, triggers, true);
        }
    }
}
