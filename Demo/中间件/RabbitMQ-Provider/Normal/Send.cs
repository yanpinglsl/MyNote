using RabbitMQ.Client;
using RabbitMQ_Consumer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQ_Provider.Noraml
{
    public class Send
    {

        public static void SendMessage()
        {
            string queueName = "normal";

            using (var connection = RabbitMQHelper.GetConnection())
            {
                // 创建信道
                using(var channel = connection.CreateModel())
                {
                    // 创建队列
                    channel.QueueDeclare(queue:queueName,durable: false, false, false, null);
                    // 没有绑定交换机，怎么找到路由队列的呢？
                    while (true)
                    {
                        string message = "Hello RabbitMQ Message";
                        var body = Encoding.UTF8.GetBytes(message);
                        // 发送消息到rabbitmq,使用rabbitmq中默认提供交换机路由,默认的路由Key和队列名称完全一致
                        channel.BasicPublish(exchange: "", routingKey: queueName, null, body);
                        Thread.Sleep(1000);
                        Console.WriteLine("Send Normal message");
                    }
                   
                }
            }
            
        } 
    }
}
