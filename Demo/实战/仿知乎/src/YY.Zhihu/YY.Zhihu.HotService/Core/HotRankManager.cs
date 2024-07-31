using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.HotService.Data;

namespace YY.Zhihu.HotService.Core
{
    public class HotRankManager(IConnectionMultiplexer redis)
    {
        private readonly IDatabase _db = redis.GetDatabase();

        private const int FollowWeight = 100;
        private const int AnswerWeight = 20;
        private const int LikeWeight = 1;
        private const int ViewWeight = 1;

        private int CalcHeatValue(QuestionStat stat)
        {
            return stat.FollowCount * FollowWeight
                   + stat.ViewCount * ViewWeight
                   + stat.AnswerCount * AnswerWeight
                   + stat.LikeCount * LikeWeight;
        }

        public async Task CreateHotRankAsync(Dictionary<int,QuestionStat> questionStats)
        {
            var sortedSetEntries = questionStats
                .Select(l => new SortedSetEntry(l.Key, CalcHeatValue(l.Value)));
            await _db.SortedSetAddAsync(RedisConstant.HotRanking, sortedSetEntries.ToArray());

        }
        public async Task UpdateHotRankAsync(Dictionary<int, QuestionStat> questionStats)
        {
            var batch = _db.CreateBatch();
            var batchTasks = new List<Task>();
            foreach (var item in questionStats)
            {
                var heatValue = CalcHeatValue(item.Value);
                if (heatValue == 0) continue;
                batchTasks.Add(
                // zincrby 无法批量更新
                //此处使用管道更新
                batch.SortedSetIncrementAsync(
                    RedisConstant.HotRanking,
                    item.Key,
                    heatValue));
            }
            if (batchTasks.Count == 0) return;
            batch.Execute();
            await Task.WhenAll(batchTasks);
        }

        public async Task ClearHotRankAsync()
        {
           await _db.KeyDeleteAsync(RedisConstant.HotRanking);
        }
    }
}
