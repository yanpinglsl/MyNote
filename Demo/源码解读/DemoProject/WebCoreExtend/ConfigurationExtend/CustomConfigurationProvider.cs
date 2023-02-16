using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCoreExtend.ConfigurationExtend
{
    public class CustomConfigurationProvider : ConfigurationProvider
    {
        private CustomConfigurationOptions _CustomConfigurationOption = null;
        public CustomConfigurationProvider(CustomConfigurationOptions customConfigurationOption)
        {
            this._CustomConfigurationOption = customConfigurationOption;
        }

        /// <summary>
        /// 数据加载方法
        /// </summary>
        public override void Load()
        {
            Console.WriteLine($"CustomConfigurationProvider load data");
            //当然也可以从数据库读取
            //var result = this._CustomConfigurationOption.DataInitFunc.Invoke();
            //this._CustomConfigurationOption.DataChangeAction()
            
            base.Data.Add("TodayCustom", "0117-Custom");
            base.Data.Add("CustomOptions:HostName", "192.168.3.254-Custom");
            base.Data.Add("CustomOptions:UserName", "guest-Custom");
            base.Data.Add("CustomOptions:Password", "guest-Custom");
        }

        public override bool TryGet(string key, out string? value)
        {
            return base.TryGet(key, out value);
        }

        public override void Set(string key, string? value)
        {
            base.Set(key, value);
        }
    }
}
