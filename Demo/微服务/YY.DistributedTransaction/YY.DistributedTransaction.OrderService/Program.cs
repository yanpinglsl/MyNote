using Microsoft.EntityFrameworkCore;
using YY.DistributedTransaction.EFModel;

namespace YY.DistributedTransaction.OrderService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            string conn = builder.Configuration.GetConnectionString("OrderServerConn");
            builder.Services.AddCap(x =>
            {
                x.UseSqlServer(conn);
                x.UseRabbitMQ(opt =>
                {
                    builder.Configuration.GetSection("RabbitMQConn").Bind(opt);
                });
                //���Ե�������
                x.FailedRetryCount = 10;
                //ÿ�����Եļ��ʱ��
                x.FailedRetryInterval = 60;
                //ʧ�����Դ�����������ʱ�����Ļص�����
                x.FailedThresholdCallback = failed =>
                {
                    string errorMsg = $@"MessageType {failed.MessageType} ʧ���ˣ� ������ {x.FailedRetryCount} ��, 
                        ��Ϣ����: {failed.Message}";//do anything
                    Console.WriteLine(errorMsg);
                };//ʧ�ܳ���������Ļص�

                //���10��---��һ���ʧ��10��---�ص���֪ͨ---�˹��ָ�������ҵ����ͨ��---Ȼ���޸����ݿ��retries
                x.TopicNamePrefix = "ZhaoxiDistributedTransaction";

                #region ע��Consul���ӻ�
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
