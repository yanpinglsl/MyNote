using RabbitMQ.Client;
using RabbitMQ_Consumer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQ_Provider.Exchange.Fanout
{
    public class FanoutSend
    {

        public static void SendMessage()
        {
            using (var connection = RabbitMQHelper.GetConnection())
            {
                using(var channel = connection.CreateModel())
                {
                    // 声明交换机对象
                    channel.ExchangeDeclare("fanout_exchange", "fanout");
                    // 创建队列
                    string queueName1 = "fanout_queue1";
                    channel.QueueDeclare(queueName1, false, false, false, null);
                    string queueName2 = "fanout_queue2";
                    channel.QueueDeclare(queueName2, false, false, false, null);
                    string queueName3 = "fanout_queue3";
                    channel.QueueDeclare(queueName3, false, false, false, null);
                    // 绑定到交互机
                    // fanout_exchange 绑定了 3个队列 
                    channel.QueueBind(queue: queueName1, exchange: "fanout_exchange", routingKey: "");
                    channel.QueueBind(queue: queueName2, exchange: "fanout_exchange", routingKey: "");
                    channel.QueueBind(queue: queueName3, exchange: "fanout_exchange", routingKey: "");

                    for (int i = 0; i < 10; i++)
                    {
                        string message = $"RabbitMQ Fanout {i + 1} Message";
                        var body = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish("fanout_exchange", "", null, body);
                        Console.WriteLine("Send Fanout {0} message",i + 1);
                    }
                }
            }
            
        } 
    }
}
