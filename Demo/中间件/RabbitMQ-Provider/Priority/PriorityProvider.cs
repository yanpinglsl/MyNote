using RabbitMQ.Client;
using RabbitMQ_Consumer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_Provider.Priority
{
    public class PriorityProvider
    {
        public static void SendMessage()
        {
            string exchange = "pro.exchange";
            string queueName = "pro.queue";
            using (var connection = RabbitMQHelper.GetConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange, type: "fanout", durable: true, autoDelete: false);
                    //x-max-priority属性必须设置，否则消息优先级不生效
                    channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, arguments: new Dictionary<string, object> { { "x-max-priority", 50 } });
                    channel.QueueBind(queueName, exchange, queueName);
                    while (true)
                    {
                        var messagestr = Console.ReadLine();
                        var messagepri = Console.ReadLine();
                        var props = channel.CreateBasicProperties();
                        props.Persistent = true;
                        props.Priority = (byte)int.Parse(messagepri);//设置消息优先级
                        channel.BasicPublish(exchange, "", true, props, Encoding.UTF8.GetBytes(messagestr));
                    }
                }
            }
        }
    }
}
