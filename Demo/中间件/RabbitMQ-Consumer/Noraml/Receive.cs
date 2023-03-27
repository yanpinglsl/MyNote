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
                    //推送的消息如何进行消费的接口回调
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received +=(model, ea) => {
                        var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                        Console.WriteLine(" Normal Received => {0}", message);
                    };
                    /**
                    * 消费者消费消息
                    * 1.消费哪个队列
                    * 2.消费成功之后是否要自动应答 true 代表自动应答 false 手动应答
                    * 3.消费者未成功消费的回调
                    */
                    channel.BasicConsume(queueName,true, consumer);
                }
            }
          
        } 
    }
}
