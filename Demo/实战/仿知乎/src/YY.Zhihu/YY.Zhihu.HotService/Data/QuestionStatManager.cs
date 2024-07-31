using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YY.Zhihu.HotService.Data
{
    public class QuestionStatManager
    {
        private readonly object _lock = new object();
        private Dictionary<int, QuestionStat> Items { get; set; } = new();
        public QuestionStatManager() { }


        public void Set(Dictionary<int, QuestionStat> item)
        {
            lock (_lock)
            {
                Items = item;
                Reset();
            }
        }

        public void Reset()
        {
            foreach(var item in Items)
            {
                item.Value.ViewCount = 0;
                item.Value.FollowCount = 0;
                item.Value.AnswerCount = 0;
                item.Value.LikeCount = 0;
            }
        }
        public Dictionary<int, QuestionStat>? GetAndReset()
        {
            if (Items.Count == 0)   return null;
            lock (_lock)
            {
                var result = Items.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value with { });
                Reset();
                return result;
            }
        }

        public void AddViewCount(int id, int count = 1)
        {
            lock (_lock)
            {
                if (!Items.TryGetValue(id, out var stat)) return;
                stat.ViewCount += count;
            }
        }
        public void AddFollowCount(int id, int count = 1)
        {
            lock (_lock)
            {
                if (!Items.TryGetValue(id, out var stat)) return;
                stat.FollowCount += count;
            }
        }
        public void AddAnswerCount(int id, int count = 1)
        {
            lock (_lock)
            {
                if (!Items.TryGetValue(id, out var stat)) return;
                stat.AnswerCount += count;
            }
        }
        public void AddLikeCount(int id, int count = 1)
        {
            lock (_lock)
            {
                if (!Items.TryGetValue(id, out var stat)) return;
                stat.LikeCount += count;
            }
        }
    }
}
