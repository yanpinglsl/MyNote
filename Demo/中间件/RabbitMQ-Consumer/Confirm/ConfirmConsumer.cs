using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_Consumer.Confirm
{
    public class ConfirmConsumer
    {
        public static void ReceiveMessage()
        {
            var connection = RabbitMQHelper.GetConnection("192.168.200.101", 5672);
            {
                var channel = connection.CreateModel();
                {
                    channel.ExchangeDeclare("confirm-exchange", ExchangeType.Direct, true);
                    channel.QueueDeclare("confirm-queue", false, false, false, null);
                    channel.QueueBind("confirm-queue", "confirm-exchange", "", null);

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
                    channel.BasicConsume(queue: "confirm-queue",
                                         autoAck: false,
                                         consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }

        }
    }
}
