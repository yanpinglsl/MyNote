using Serilog.Events;
using Serilog;

namespace SerilogDemo
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                // ������=���/ɾ�����޸ĸ��ӵ���־�¼�������
                // ABP MVC ģ����־����
                .Enrich.FromLogContext()
                .WriteTo.Async(c => c.File("Logs/logs.txt"))
                .WriteTo.Async(c => c.Console())
                .CreateLogger();

            // var configuration = new ConfigurationBuilder()
            //     .SetBasePath(Directory.GetCurrentDirectory())
            //     .AddJsonFile("appsettings.json")
            //     .Build();
            //
            // Log.Logger = new LoggerConfiguration()
            //     .ReadFrom.Configuration(configuration)
            //     .CreateLogger();


            try
            {
                Log.Information("Starting web host.");
                var builder = WebApplication.CreateBuilder(args);
                builder.Host.UseSerilog();
                await builder.AddApplicationAsync<AppModule>();
                var app = builder.Build();
                await app.InitializeApplicationAsync();
                await app.RunAsync();
                return 0;
            }
            catch (Exception ex)
            {
                if (ex is HostAbortedException)
                {
                    throw;
                }

                Log.Fatal(ex, "Host terminated unexpectedly!");
                return 1;
            }
            finally
            {
                await Log.CloseAndFlushAsync();
            }
        }


    }
}
