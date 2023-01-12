using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_Consumer.Exchange.Fanout
{
    public class FanoutConsumer
    {
        public static void ConsumerMessage()
        {
            var connection = RabbitMQHelper.GetConnection();
            {
                var channel = connection.CreateModel();
                {
                    //申明exchange
                    channel.ExchangeDeclare(exchange: "fanout_exchange", type: "fanout");
                    // 创建队列
                    string queueName1 = "fanout_queue1";
                    channel.QueueDeclare(queueName1, false, false, false, null);
                    string queueName2 = "fanout_queue2";
                    channel.QueueDeclare(queueName2, false, false, false, null);
                    string queueName3 = "fanout_queue3";
                    channel.QueueDeclare(queueName3, false, false, false, null);
                    // 绑定到交互机
                    channel.QueueBind(queue: queueName1, exchange: "fanout_exchange", routingKey: "");
                    channel.QueueBind(queue: queueName2, exchange: "fanout_exchange", routingKey: "");
                    channel.QueueBind(queue: queueName3, exchange: "fanout_exchange", routingKey: "");


                    Console.WriteLine("[*] Waitting for fanout logs.");
                    //申明consumer
                    var consumer = new EventingBasicConsumer(channel);
                    //绑定消息接收后的事件委托
                    consumer.Received += (model, ea) => {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body.ToArray());
                        Console.WriteLine("[x] {0}", message);

                    };

                    channel.BasicConsume(queue: queueName3, autoAck: true, consumer: consumer);
                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}
