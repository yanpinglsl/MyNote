using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;

namespace VirtualFileDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var application = AbpApplicationFactory.Create<SampleAbpModule>();

            application.Initialize();

            var service =
                application.ServiceProvider.GetService<SampleService>();

            service.Test();
        }
    }
}
