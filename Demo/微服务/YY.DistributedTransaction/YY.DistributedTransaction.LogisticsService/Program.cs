using Microsoft.EntityFrameworkCore;
using YY.DistributedTransaction.EFModel;

namespace YY.DistributedTransaction.LogisticsService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            string conn = builder.Configuration.GetConnectionString("LogisticsServerConn");
            builder.Services.AddCap(x =>
            {
                x.UseSqlServer(conn);
                x.UseRabbitMQ(opt =>
                {
                    builder.Configuration.GetSection("RabbitMQConn").Bind(opt);
                });
                //重试的最大次数
                x.FailedRetryCount = 10;
                //每次重试的间隔时间
                x.FailedRetryInterval = 60;
                //失败重试次数到达上限时触发的回调函数
                x.FailedThresholdCallback = failed =>
                {
                    string errorMsg = $@"MessageType {failed.MessageType} 失败了， 重试了 {x.FailedRetryCount} 次, 
                        消息名称: {failed.Message}";//do anything
                    Console.WriteLine(errorMsg);
                };//失败超出次数后的回调

                //最高10次---万一真的失败10次---回调发通知---人工恢复数据让业务能通过---然后修改数据库的retries
                x.TopicNamePrefix = "ZhaoxiDistributedTransaction";

                #region 注册Consul可视化
                //x.UseDashboard();
                //DiscoveryOptions discoveryOptions = new DiscoveryOptions();
                //this.Configuration.Bind(discoveryOptions);
                //x.UseDiscovery(d =>
                //{
                //    d.DiscoveryServerHostName = discoveryOptions.DiscoveryServerHostName;
                //    d.DiscoveryServerPort = discoveryOptions.DiscoveryServerPort;
                //    d.CurrentNodeHostName = discoveryOptions.CurrentNodeHostName;
                //    d.CurrentNodePort = discoveryOptions.CurrentNodePort;
                //    d.NodeId = discoveryOptions.NodeId;
                //    d.NodeName = discoveryOptions.NodeName;
                //    d.MatchPath = discoveryOptions.MatchPath;
                //});
                #endregion
                x.UseEntityFramework<CommonServiceDbContext>();
                x.UseDashboard();
            });

            #region EFCore
            builder.Services.AddDbContext<CommonServiceDbContext>(options =>
            {
                options.UseSqlServer(conn);
            });
            #endregion
            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
