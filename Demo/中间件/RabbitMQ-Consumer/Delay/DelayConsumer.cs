using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_Consumer.Delay
{
    public class DelayConsumer
    {
        public static void ReceiveMessage()
        {
            var connection = RabbitMQHelper.GetConnection("192.168.3.10", 5672);
            {
                var channel = connection.CreateModel();
                {
                    channel.ExchangeDeclare(exchange: "exchange-direct", type: "direct");
                    string name = channel.QueueDeclare().QueueName;
                    channel.QueueBind(queue: name, exchange: "exchange-direct", routingKey: "routing-delay");

                    //回调，当consumer收到消息后会执行该函数
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(ea.RoutingKey);
                        Console.WriteLine(" [x] Received {0}", message);
                    };

                    //Console.WriteLine("name:" + name);
                    //消费队列"hello"中的消息
                    channel.BasicConsume(queue: name,
                                         autoAck: true,
                                         consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }

        }
    }
}
