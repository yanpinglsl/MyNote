using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_Consumer.Delay
{
    public class DelayPluginConsumer
    {
        public static void ReceiveMessage()
        {
            var exchangePlugin = "plug.delay.exchange";
            var routedlxPlugin = "plugdelay";
            var queuedlxPlugin = "plug.delay.queue";
            //创建连接
            using (var connection = RabbitMQHelper.GetConnection())
            {
                //创建通道
                using (var channel = connection.CreateModel())
                {   
                    //业务的交换机和队列绑定
                    channel.ExchangeDeclare(exchangePlugin, "x-delayed-message", true, false, arguments:
                             new Dictionary<string, object>()
                             {
                                    {"x-delayed-type","direct" }
                             }
                    );
                    channel.QueueDeclare(queuedlxPlugin, true, false, false, null);
                    channel.QueueBind(queuedlxPlugin, exchangePlugin, routedlxPlugin);
                    //回调，当consumer收到消息后会执行该函数
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                        //打印消费的消息
                        Console.WriteLine(message);
                        channel.BasicAck(ea.DeliveryTag, false);
                    };

                    //消费queue.business.test队列的消息
                    channel.BasicConsume(queuedlxPlugin, false, consumer);

                    Console.ReadLine();
                }
            }

        }
    }
}
