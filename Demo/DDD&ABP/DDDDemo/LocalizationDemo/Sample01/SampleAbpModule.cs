using LocalizationDemo.Localization.Sample;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace LocalizationDemo.Sample01
{
    [DependsOn(typeof(AbpLocalizationModule))]
    public class SampleAbpModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<SampleAbpModule>(nameof(LocalizationDemo));
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Add<SampleResource>("zh-Hans")//默认语言
                    .AddVirtualJson("/Localization/Sample/Resources");

                options.DefaultResourceType = typeof(SampleResource);
            });
        }
    }


}
