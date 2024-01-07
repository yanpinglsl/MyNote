using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Validation;

namespace ModelValidatorDemo.Sample02
{
    public class SampleService : ITransientDependency
    {
        private readonly IObjectValidator _objectValidator;

        public SampleService(IObjectValidator objectValidator)
        {
            _objectValidator = objectValidator;
        }

        public virtual async Task Test(CreateProductDto input)
        {
            try
            {
                await _objectValidator.ValidateAsync(input);
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
