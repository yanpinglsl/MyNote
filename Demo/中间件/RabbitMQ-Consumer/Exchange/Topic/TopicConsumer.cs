using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_Consumer.Exchange.Topic
{
    public class TopicConsumer
    {
        public static void ConsumerMessage()
        {
            var connection = RabbitMQHelper.GetConnection();
            var channel = connection.CreateModel();
            var queueName = "topic_queue1";
            channel.ExchangeDeclare(exchange: "topic_exchange", type: "topic");
            channel.QueueDeclare(queueName, false, false, false, null);
            // 有个bug
            channel.QueueBind(queue: queueName,
                                      exchange: "topic_exchange",
                                      routingKey: "user.data.insert");

            Console.WriteLine(" [*] Waiting for messages.");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());
                var routingKey = ea.RoutingKey;
                Console.WriteLine(" [x] Received '{0}':'{1}'", routingKey, message);
            };

            channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
