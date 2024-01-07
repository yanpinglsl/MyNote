using System;
using System.Threading.Tasks;
using ModelValidatorDemo.Sample03;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Settings;
using Volo.Abp.Validation;

namespace ModelValidatorDemo.Sample01
{
    
    public class SampleService : ITransientDependency, IValidationEnabled
    {
        public virtual void Test(CreateProductDto input)
        {
            Console.WriteLine(input.Name);
        }
    }
}
