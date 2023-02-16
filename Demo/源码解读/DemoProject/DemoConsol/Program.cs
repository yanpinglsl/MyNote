using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace DemoConsol
{
    internal class Program
    {
        static void Main(string[] args)
        {
            WatchFor();
            Console.WriteLine("Hello, World!");
        }

        public static void WatchOne()
        {
            PhysicalFileProvider phyFileProvider = new PhysicalFileProvider(AppContext.BaseDirectory);
            var changeToken = phyFileProvider.Watch("changetoken.json");//监听，返回的就是IChangeToken

            changeToken.RegisterChangeCallback(state =>
            {
                Console.WriteLine("文件被更新！！！");
            }, new object());

            Console.ReadKey();
        }

        public static void WatchFor()
        {
            PhysicalFileProvider phyFileProvider = new PhysicalFileProvider(AppContext.BaseDirectory);
            ChangeToken.OnChange(() => phyFileProvider.Watch("changetoken.json"), () =>
            {
                Console.WriteLine("文件被更新！！！");
            });
            Console.ReadKey();
        }
    }
}