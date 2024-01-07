using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.VirtualFileSystem;

namespace VirtualFileDemo
{

    public class SampleService : ITransientDependency
    {
        private readonly IVirtualFileProvider _virtualFileProvider;

        public SampleService(IVirtualFileProvider virtualFileProvider)
        {
            _virtualFileProvider = virtualFileProvider;
        }

        public void Test()
        {

            var file = _virtualFileProvider
                .GetFileInfo("/MyResources/Hello.txt");

            if (file.Exists)
            {
                var fileContent = file.ReadAsString();
                Console.WriteLine(fileContent);
            }
            else
            {
                Console.WriteLine("文件不存在");
            }


        }
    }
}
