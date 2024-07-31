using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YY.Zhihu.UseCases.Questions.Jobs
{
    public class QuestionViewCountService
    {
        private readonly object Lock = new object();
        private Dictionary<int, int> Item = new Dictionary<int, int>();
        public void AddViewCount(int id, int count = 1)
        {
            lock (Lock)
            {
                if (!Item.TryAdd(id, count))
                {
                    Item[id] += count;
                }
            }
        }
        public Dictionary<int, int>? GetAndReset()
        {
            if (Item.Count == 0) return null;
            lock (Lock)
            {
                var result = new Dictionary<int, int>(Item);
                Item.Clear();
                return result;
            }
        }
    }
}
