using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_Consumer.Durable
{
    public class DurableConsumer
    {
        public static void ReceiveMessage()
        {
            var connection = RabbitMQHelper.GetConnection("192.168.3.10", 5672);
            {
                var channel = connection.CreateModel();
                {
                    //1、 创建持久化队列
                    channel.QueueDeclare("durable_queue", true, false, false, null);
                    //2、 创建持久化的交换机
                    channel.ExchangeDeclare("durable_exchange", "fanout", true, false, null);
                    channel.QueueBind("durable_queue", "durable_exchange", "", null);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) => {
                        var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                        Console.WriteLine(" Durable Queue Received => {0}", message);
                    };
                    channel.BasicConsume("durable_queue", true, consumer);
                }
            }

        }
    }
}
