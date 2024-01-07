using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;

namespace ModelValidatorDemo.Sample02
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
