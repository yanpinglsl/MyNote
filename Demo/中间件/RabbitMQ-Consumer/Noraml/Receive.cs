using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQ_Consumer.Noraml
{
    public class Receive
    {
        public static void ReceiveMessage()
        {
            // 消费者消费是队列中消息
            string queueName = "normal";
            var connection = RabbitMQHelper.GetConnection("172.17.6.21", 5672);
            {
                var channel = connection.CreateModel();
                {
                    channel.QueueDeclare(queueName, false, false, false, null);
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received +=(model, ea) => {
                        var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                        Console.WriteLine(" Normal Received => {0}", message);
                    }; 
                    channel.BasicConsume(queueName,true, consumer);
                }
            }
          
        } 
    }
}
