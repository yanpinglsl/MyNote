using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace VirtualFileDemo
{
    [DependsOn(typeof(AbpVirtualFileSystemModule))]
    public class SampleAbpModule: AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<SampleAbpModule>(
                    nameof(VirtualFileDemo),
                    "MyResources");
            });
        }
    }
}
