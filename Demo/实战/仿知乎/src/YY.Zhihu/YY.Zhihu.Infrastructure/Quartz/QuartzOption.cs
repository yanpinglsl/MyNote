using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YY.Zhihu.Infrastructure.Quartz
{

    public class QuartzOption
    {
        public SchedulerOption[] Schedulers { get; init; } = null!;
    }

    public class SchedulerOption : Dictionary<string, string?>
    {
        public NameValueCollection ToNameValueCollection()
        {
            var collection = new NameValueCollection(Count);
            foreach (var pair in this)
            {
                collection[pair.Key] = pair.Value;
            }

            return collection;
        }
    }

}
