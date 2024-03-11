using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_Consumer.Exchange.Direct
{
    public class DirectConsumer
    {
        public static void ConsumerMessage()
        {
            var connection = RabbitMQHelper.GetConnection();
            var channel = connection.CreateModel();
            channel.ExchangeDeclare(exchange: "direct_exchange", type: "direct");
            var queueName = "direct_queue1";
            channel.QueueDeclare(queueName, false, false, false, null);
            channel.QueueBind(queue: queueName,
                                      exchange: "direct_exchange",
                                      routingKey: "red");
            channel.QueueBind(queue: queueName,
                                      exchange: "direct_exchange",
                                      routingKey: "yellow");
            channel.QueueBind(queue: queueName,
                                      exchange: "direct_exchange",
                                      routingKey: "green");

            Console.WriteLine(" [*] Waiting for messages.");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());
                var routingKey = ea.RoutingKey;
                Console.WriteLine(" [x] Received '{0}':'{1}'", routingKey, message);

                // 消费完成后需要手动签收消息，如果不写该代码就容易导致重复消费问题
                channel.BasicAck(ea.DeliveryTag, true); // 可以降低每次签收性能损耗
            };

            // 消息签收模式
            // 手动签收 保证正确消费，不会丢消息(基于客户端而已)
            // 自动签收 容易丢消息 
            // 签收：意味着消息从队列中删除
            channel.BasicConsume(queue: queueName,
                                 autoAck: false,
                                 consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
