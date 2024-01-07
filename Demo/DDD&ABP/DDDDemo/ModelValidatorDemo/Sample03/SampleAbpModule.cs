using Volo.Abp.Autofac;
using Volo.Abp.FluentValidation;
using Volo.Abp.Modularity;

namespace ModelValidatorDemo.Sample03
{
    [DependsOn(typeof(AbpAutofacModule),
        typeof(AbpFluentValidationModule))]
    public class SampleAbpModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {

        }
    }
}
