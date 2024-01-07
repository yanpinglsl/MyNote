using System;
using System.Globalization;
using LocalizationDemo.Localization.Sample;
using Microsoft.Extensions.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Localization;

namespace LocalizationDemo.Sample01
{
    public class SampleService : ITransientDependency
    {
        private readonly IStringLocalizer<SampleResource> _localizer;

        public SampleService(IStringLocalizer<SampleResource> localizer)
        {
            _localizer = localizer;
        }

        public void Test()
        {
            //不存在的语言
            CultureHelper.Use("cs");
            Console.WriteLine("当前地区：" + CultureInfo.CurrentCulture.Name);
            Console.WriteLine(_localizer["HelloMessage", "张三"]);

            //默认语言
            Console.WriteLine("当前地区：" + CultureInfo.CurrentCulture.Name);
            var str = _localizer["HelloMessage", "张三"];
            Console.WriteLine(str);

            //切换语言
            CultureHelper.Use("en");
            Console.WriteLine("当前地区：" + CultureInfo.CurrentCulture.Name);
            Console.WriteLine(_localizer["HelloMessage", "张三"]);
            Console.ReadLine();
        }
    }
}
