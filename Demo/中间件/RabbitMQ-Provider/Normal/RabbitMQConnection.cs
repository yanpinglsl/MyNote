using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_Provider.Normal
{
    public class RabbitMQConnection
    {
        public static void SendMessage()
        {
            // 创建工厂对象
            var connectionFactory = new ConnectionFactory()
            {
                HostName = "192.168.1.102",
                Port = 5672,
                UserName = "gerry",
                Password = "gerry",
                VirtualHost = "/"
            };
            // 通过工厂对象创建连接对象
            var connection = connectionFactory.CreateConnection();
            // 通过连接对象获取Channel对象
            var channel = connection.CreateModel();
            Console.WriteLine(channel);
        
        }
    }
}
