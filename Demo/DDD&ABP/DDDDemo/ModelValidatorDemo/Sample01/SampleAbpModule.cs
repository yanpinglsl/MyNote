using Volo.Abp.Autofac;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;

namespace ModelValidatorDemo.Sample01
{
    [DependsOn(typeof(AbpAutofacModule),
        typeof(AbpValidationModule))]
    public class SampleAbpModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {

        }
    }
}
