using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.Domain.QuestionAggerate.Entites;
using YY.Zhihu.HotService.Core;
using YY.Zhihu.HotService.Data;

namespace YY.Zhihu.HotService.Jobs
{
    public class UpdateHotRankJob(
        QuestionStatManager questionStatManager,
        HotRankManager hotRankManager) : IJob
    {
        public static JobKey Key = new JobKey(nameof(UpdateHotRankJob), nameof(HotService));
        public async Task Execute(IJobExecutionContext context)
        {
            var questionStats = questionStatManager.GetAndReset();
            if (questionStats == null) return;
            await hotRankManager.UpdateHotRankAsync(questionStats);
        }
    }
}
