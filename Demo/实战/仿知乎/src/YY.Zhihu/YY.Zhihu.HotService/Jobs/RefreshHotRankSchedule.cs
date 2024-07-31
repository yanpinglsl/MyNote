using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.Domain.QuestionAggerate.Entites;

namespace YY.Zhihu.HotService.Jobs
{
    public static class RefreshHotRankSchedule
    {
        public static TriggerKey Key = new TriggerKey(nameof(RefreshHotRankSchedule), nameof(HotService));

        //使用Quartz提供的依赖注入只支持单调度器
        //NuGet：Quartz.Extensions.DependencyInjection
        //public static void CreateRefreshHotRankJobSchedule(
        //    this IServiceCollectionQuartzConfigurator configurator,
        //    int hour = 5)
        //{
        //    configurator.AddJob<RefreshHotRankJob>(triggerConfigurator => triggerConfigurator
        //        .WithIdentity(RefreshHotRankJob.Key)
        //        .RequestRecovery());

        //    configurator.AddTrigger(trigger => trigger
        //        .WithIdentity(Key)
        //        .ForJob(RefreshHotRankJob.Key)
        //        .WithCronSchedule($"0 0 {hour} * * ?"));

        //    configurator.AddTrigger(trigger => trigger
        //        .ForJob(RefreshHotRankJob.Key)
        //        .StartNow());
        //}


        //使用Quartz组件自定义多调度器
        //NuGet：Quartz.Jobs
        public static void CreateRefreshHotRankJobSchedule(
            this IScheduler scheduler,
            int hour = 5)
        {
            var jobDetail = JobBuilder.Create<RefreshHotRankJob>()
                .WithIdentity(RefreshHotRankJob.Key)
                .Build();

            var triggers = new List<ITrigger>
            {
                TriggerBuilder.Create()
                .WithIdentity(Key)
                .ForJob(RefreshHotRankJob.Key)
                .WithCronSchedule($"0 0 {hour} * * ?")
                .Build(),
                
                TriggerBuilder.Create()
                .ForJob(RefreshHotRankJob.Key)
                .StartNow()
                .Build(),
            };
            scheduler.ScheduleJob(jobDetail, triggers, true);
        }
    }
}
