using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_Provider.Confirm
{
    public class Transaction
    {
        /// <summary>
        /// 使用事务方式确保数据正确到达消息服务端
        /// </summary>
        public static void TransactionMode()
        {
            ConnectionFactory factory = new ConnectionFactory { HostName = "192.168.200.101", UserName = "yp", Password = "yp", VirtualHost = "/" };
            using (IConnection conn = factory.CreateConnection())
            {
                using (IModel im = conn.CreateModel())
                {
                    try
                    {
                        im.TxSelect(); //用于将当前channel设置成transaction事务模式
                        im.ExchangeDeclare("my-exchange", ExchangeType.Direct);
                        im.QueueDeclare("my-queue", true, false, false, null);
                        im.QueueBind("my-queue", "my-exchange", "", null);
                        var properties = im.CreateBasicProperties();
                        properties.DeliveryMode = 2;
                        Console.Write("输入发送的内容：");
                        var msg = Console.ReadLine();
                           
                        byte[] message = Encoding.UTF8.GetBytes("发送消息:" + msg);
                        im.BasicPublish("my-exchange", ExchangeType.Direct, properties, message);
                        im.TxCommit();//txCommit用于提交事务
                    }
                    catch (Exception ex)
                    {
                        im.TxRollback();
                    }
                }
            }
        }
    }
}
