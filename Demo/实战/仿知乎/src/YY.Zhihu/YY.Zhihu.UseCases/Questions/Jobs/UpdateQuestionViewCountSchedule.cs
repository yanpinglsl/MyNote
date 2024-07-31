using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.Domain.QuestionAggerate.Entites;
using YY.Zhihu.Domain.QuestionAggerate.Specifications;
using YY.Zhihu.HotService.Jobs;
using YY.Zhihu.SharedLibraries.Repositoy;

namespace YY.Zhihu.UseCases.Questions.Jobs
{
    public static class UpdateQuestionViewCountSchedule
    {
        private static TriggerKey Key = new TriggerKey(nameof(UpdateQuestionViewCountSchedule), nameof(Question));

        //使用Quartz提供的依赖注入只支持单调度器
        //NuGet：Quartz.Extensions.DependencyInjection
        //public static void CreateUpdateQuestionViewCountJobSchedule(
        //    this IServiceCollectionQuartzConfigurator configurator,
        //    int intervalTime = 20)
        //{
        //    configurator.AddJob<UpdateQuestionViewCountJob>(triggerConfigurator => triggerConfigurator
        //        .WithIdentity(UpdateQuestionViewCountJob.Key));

        //    configurator.AddTrigger(trigger => trigger
        //        .WithIdentity(Key)
        //        .ForJob(UpdateQuestionViewCountJob.Key)
        //        .StartAt(DateBuilder.FutureDate(intervalTime, IntervalUnit.Second))
        //        .WithSimpleSchedule(builder => builder
        //            .RepeatForever()
        //            .WithInterval(TimeSpan.FromSeconds(intervalTime))
        //        ));

        //}

        //使用Quartz组件自定义多调度器
        //NuGet：Quartz.Jobs
        public static void CreateUpdateQuestionViewCountJobSchedule(
          this IScheduler scheduler,
          int intervalTime = 20)
        {
            var jobDetails = JobBuilder.Create<UpdateQuestionViewCountJob>()
                .WithIdentity(RefreshHotRankJob.Key)
                .Build();

            var triggers = new List<ITrigger>
            {
                TriggerBuilder.Create()
                .WithIdentity(Key)
                .ForJob(UpdateQuestionViewCountJob.Key)
                .StartAt(DateBuilder.FutureDate(intervalTime, IntervalUnit.Second))
                .WithSimpleSchedule(builder => builder
                    .RepeatForever()
                    .WithInterval(TimeSpan.FromSeconds(intervalTime))                )
                .Build()
            };
            scheduler.ScheduleJob(jobDetails, triggers, true);
        }
    }
}
