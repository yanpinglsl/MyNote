using Quartz;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.Domain.QuestionAggerate.Entites;
using YY.Zhihu.HotService.Core;
using YY.Zhihu.HotService.Data;
using YY.Zhihu.UseCases.Contracts.Interfaces;

namespace YY.Zhihu.HotService.Jobs
{
    public class RefreshHotRankJob(
        ISchedulerFactory schedulerFactory,
        QuestionStatQuery questionStatQuery,
        QuestionStatManager questionStatManager,
        HotRankManager hotRankManager) : IJob
    {
        public static JobKey Key = new JobKey(nameof(RefreshHotRankJob), nameof(HotService));
        public async Task Execute(IJobExecutionContext context)
        {
            var result = await questionStatQuery.GetLatest();
            if (!result.IsSuccess) return;
            var questionStats = result.Value!;
            //单调度器
            //var schedulers = await schedulerFactory.GetScheduler();
            //await schedulers.PauseTrigger(UpdateHotRankSchedule.Key);
            //多调度器
            var triggerKey = GroupMatcher<TriggerKey>.GroupEquals(nameof(UpdateHotRankJob));
            await context.Scheduler.PauseTriggers(triggerKey);

            await hotRankManager.ClearHotRankAsync();
            await hotRankManager.CreateHotRankAsync(questionStats);

            questionStatManager.Set(questionStats);
            //单调度器
            //await schedulers.ResumeTrigger(UpdateHotRankSchedule.Key);
            //多调度器
            await context.Scheduler.ResumeTriggers(triggerKey);
        }
    }
}
