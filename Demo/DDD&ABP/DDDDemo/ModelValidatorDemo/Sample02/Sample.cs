using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ModelValidatorDemo.Sample03;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Validation;

namespace ModelValidatorDemo.Sample02
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

            await service.Test(new CreateProductDto());
            
        }
    }
}
