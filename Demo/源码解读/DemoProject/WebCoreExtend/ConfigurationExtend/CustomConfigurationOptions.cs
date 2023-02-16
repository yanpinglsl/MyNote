using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCoreExtend.ConfigurationExtend
{
    public class CustomConfigurationOptions
    {
        public string LogTag { get; set; }

        /// <summary>
        /// 数据获取方式
        /// </summary>
        public Func<IDictionary<string, string>> DataInitFunc { get; set; }

        /// <summary>
        /// 数据更新方式
        /// </summary>
        public Action<string, string> DataChangeAction { get; set; }
    }
}
