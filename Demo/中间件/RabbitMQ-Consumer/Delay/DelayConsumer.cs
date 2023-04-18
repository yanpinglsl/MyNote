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
            var exchangetest = "exchange.business.test";
            var routetest = "businessRoutingkey";
            var queuetest = "queue.business.test";
            //创建连接
            using (var connection = RabbitMQHelper.GetConnection())
            {
                //创建通道
                using (var channel = connection.CreateModel())
                {                  //业务的交换机和队列绑定
                    channel.ExchangeDeclare(exchangetest, "direct", true, false, null);
                    channel.QueueDeclare(queuetest, true, false, false, null);
                    channel.QueueBind(queuetest, exchangetest, routetest, null);

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
                    channel.BasicConsume(queuetest, false, consumer);

                    Console.ReadLine();
                }
            }

        }
    }
}
