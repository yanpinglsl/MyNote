﻿using LocalizationDemo.Localization.Sample;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace LocalizationDemo.Sample02
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
                    .Add<SampleResource>("zh-Hans")
                    .AddVirtualJson("/Localization/Sample/Resources");

                options.DefaultResourceType = typeof(SampleResource);
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Languages.Add(new LanguageInfo("en", "en", "English"));
                options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));
                options.Languages.Add(new LanguageInfo("zh-Hant", "zh-Hant", "繁體中文"));
            });
        }
    }


}
