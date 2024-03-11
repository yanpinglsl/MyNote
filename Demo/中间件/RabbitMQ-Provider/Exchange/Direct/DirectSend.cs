using RabbitMQ.Client;
using RabbitMQ_Consumer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQ_Provider.Exchange.Direct
{
    public class DirectSend
    {

        public static void SendMessage()
        {
            using (var connection = RabbitMQHelper.GetConnection())
            {
                using(var channel = connection.CreateModel())
                {
                    // 声明Direct交换机
                    channel.ExchangeDeclare("direct_exchange", "direct");
                    // 创建队列
                    string queueName1 = "direct_queue1";
                    channel.QueueDeclare(queueName1, false, false, false, null);
                    string queueName2 = "direct_queue2";
                    channel.QueueDeclare(queueName2, false, false, false, null);
                    string queueName3 = "direct_queue3";
                    channel.QueueDeclare(queueName3, false, false, false, null);
                    // 绑定到交互机 指定routingKey
                    channel.QueueBind(queue: queueName1, exchange: "direct_exchange", routingKey: "red");
                    channel.QueueBind(queue: queueName2, exchange: "direct_exchange", routingKey: "yellow");
                    channel.QueueBind(queue: queueName3, exchange: "direct_exchange", routingKey: "green");

                    for (int i = 0; i < 10; i++)
                    {
                        string message = $"RabbitMQ Direct {i + 1} Message =>yellow";
                        var body = Encoding.UTF8.GetBytes(message);
                        // 发送消息的时候需要指定routingKey发送
                        channel.BasicPublish(exchange: "direct_exchange", routingKey: "green", null, body);
                        Console.WriteLine("Send Direct {0} message",i + 1);
                    }
                }
            }
            
        } 
    }
}
