using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;

namespace ModelValidatorDemo.Sample03
{
    public static class Sample
    {
        public static async Task Run()
        {
            var application = await AbpApplicationFactory.CreateAsync<SampleAbpModule>(options =>
            {
                options.UseAutofac();
            });

            await application.InitializeAsync();

            var service = 
                application.ServiceProvider.GetService<SampleService>();

            await service.Test(new Product());
            
        }
    }
}
