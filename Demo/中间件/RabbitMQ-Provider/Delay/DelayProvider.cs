using RabbitMQ.Client;
using RabbitMQ_Consumer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace RabbitMQ_Provider.Delay
{
    public class DelayProvider
    {
        public static void SendMessage()
        {
            var exchangedlx = "exchange.business.dlx";
            var routedlx = "";
            var queuedlx = "queue.business.dlx";

            var exchangetest = "exchange.business.test";
            var routetest = "businessRoutingkey";
            var queuetest = "queue.business.test";

            //创建连接
            using (var connection = RabbitMQHelper.GetConnection())
            {
                //创建通道
                using (var channel = connection.CreateModel())
                {
                    //延时的交换机和队列绑定
                    channel.ExchangeDeclare(exchangedlx, "direct", true, false, null);
                    channel.QueueDeclare(queuedlx, true, false, false, arguments:
                        /// 指定队列的x-dead-letter-exchange和x-dead-letter-routing -key
                        new Dictionary<string, object>()
                        {
                             {"x-dead-letter-exchange",exchangetest },
                             {"x-dead-letter-routing-key",routetest},   
                             //{ "x-message-ttl",10000} //队列TTL，设置消息的存活时间，即过期时间
                        }
                     );
                    channel.QueueBind(queuedlx, exchangedlx, routedlx);

                    //业务的交换机和队列绑定
                    channel.ExchangeDeclare(exchangetest, "direct", true, false, null);
                    channel.QueueDeclare(queuetest, true, false, false, null);
                    channel.QueueBind(queuetest, exchangetest, routetest, null);

                    Console.WriteLine("生产者开始发送消息");
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;
                    properties.Expiration = "10000";//消息TTL
                    for (int i = 0; i < 10; i++)
                    {
                        //发布消息
                        var message = string.Format("{0}:message", i);
                        Console.WriteLine("Send Direct {0} message", i);
                        var body = Encoding.UTF8.GetBytes(message);
                        //发送一条延时10秒的消息
                        channel.BasicPublish(exchangedlx, routedlx, properties, body);

                    }

                    ////如果存在A、B消息进入了队列中，A在前，B在后，
                    ////如果B消息的过期时间比A的过期时间要早，消费的时候，并不会先消费B，再消费A，
                    ////而是B会等A先消费，即使A要晚过期
                    //string message1 = "Hello Word!1";
                    //string message2 = "Hello Word!2";
                    //var body1 = Encoding.UTF8.GetBytes(message1);
                    //var body2 = Encoding.UTF8.GetBytes(message2);
                    //var properties = channel.CreateBasicProperties();
                    //properties.Persistent = true;
                    ////先发送过期时间5秒的消息
                    //properties.Expiration = "20000";
                    //channel.BasicPublish(exchangedlx, "", properties, body1);

                    ////再发送过期时间3秒的消息
                    //properties.Expiration = "10000";
                    //channel.BasicPublish(exchangedlx, "", properties, body2);

                }
            }
            Console.ReadLine();
        }
    }
}
