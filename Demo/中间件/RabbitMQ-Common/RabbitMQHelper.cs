using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_Consumer
{
    public class RabbitMQHelper
    {
        /// <summary>
        /// 获取单个RabbitMQ连接
        /// </summary>
        /// <returns></returns>
        public static IConnection GetConnection()
        {
            var factory = new ConnectionFactory
            {
                HostName = "192.168.1.102", //ip
                Port = 5672, // 端口
                UserName = "yp", // 账户
                Password = "yp", // 密码
                VirtualHost = "/"   // 虚拟主机
            };
  
            return factory.CreateConnection();
        }

        /// <summary>
        /// 根据hostName获取单个RabbitMQ连接
        /// </summary>
        /// <returns></returns>
        public static IConnection GetConnection(string hostName,int port)
        {
            var factory = new ConnectionFactory
            {
                HostName = hostName, //ip
                Port = port, // 端口
                UserName = "yp", // 账户
                Password = "yp", // 密码
                VirtualHost = "/"   // 虚拟主机
            };

            return factory.CreateConnection();
        }

        /// <summary>
        /// 获取集群连接对象
        /// </summary>
        /// <returns></returns>
        public static IConnection GetClusterConnection()
        {
            var factory = new ConnectionFactory
            {
                UserName = "yp", // 账户
                Password = "yp", // 密码
                VirtualHost = "/"   // 虚拟主机
            };
            // 
            List<AmqpTcpEndpoint> list = new List<AmqpTcpEndpoint>
            {
                new AmqpTcpEndpoint() { HostName = "192.168.3.10", Port = 5672 },
                new AmqpTcpEndpoint() { HostName = "192.168.3.10", Port = 5673 },
                new AmqpTcpEndpoint() { HostName = "192.168.3.10", Port = 5674 }
            };

            return factory.CreateConnection(list);
        }

        /// <summary>
        /// 获取集群连接对象
        /// </summary>
        /// <param name="connectStr">传入需要连接集群的字符串[eg: ip:port,ip:port,.....]</param>
        /// <returns></returns>
        public static IConnection GetClusterConnection(string connectStr)
        {
            var factory = new ConnectionFactory
            {
                UserName = "yp", // 账户
                Password = "yp", // 密码
                VirtualHost = "/"   // 虚拟主机
            };
            // 
            List<AmqpTcpEndpoint> list = new();
            string[] connectStrings = connectStr.Split(",");
            foreach (var connect in connectStrings)
            {
                string[] address = connect.Split(":");
                AmqpTcpEndpoint amqpTcpEndpoint = new(address[0], int.Parse(address[1]));
                list.Add(amqpTcpEndpoint);
            }

            return factory.CreateConnection(list);
        }
    }
}
