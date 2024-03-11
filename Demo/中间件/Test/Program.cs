namespace Test
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            await AAA();
        }
        public static async Task AAA()
        {
          await  TestAsyncNoRecall();
        }
        public static async Task TestAsyncNoRecall()
        {
            Console.WriteLine("异步方法开始");
            await Task.Run(() =>
            {
                Thread.Sleep(1000);
                Console.WriteLine("异步线程执行");
            });
            Console.WriteLine("异步方法结束");
        }
    }
}
