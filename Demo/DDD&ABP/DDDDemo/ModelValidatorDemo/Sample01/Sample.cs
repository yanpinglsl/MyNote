using System;
using System.Collections.Generic;
using System.Globalization;
using ModelValidatorDemo.Sample03;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Localization;
using Volo.Abp.Validation;

namespace ModelValidatorDemo.Sample01
{
    public static class Sample
    {
        public static void Run()
        {
            var application = AbpApplicationFactory.Create<SampleAbpModule>(options =>
            {
                options.UseAutofac();
            });

            application.Initialize();

            var service = 
                application.ServiceProvider.GetService<SampleService>();

            try
            {
                service.Test(new CreateProductDto());
            }
            catch (AbpValidationException e)
            {
                foreach (var validationError in e.ValidationErrors)
                {
                    Console.WriteLine($"MemberNames:{validationError.MemberNames.JoinAsString(",")};  ErrorMessage:{validationError.ErrorMessage}");
                    
                }
            }
            
        }
    }
}
