using RabbitMQ.Client;
using RabbitMQ_Consumer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_Provider.Delay
{
    public class DelayPluginProvider
    {
        public static void SendMessage()
        {
            var exchangePlugin = "plug.delay.exchange";
            var routedlxPlugin = "plugdelay";
            var queuedlxPlugin = "plug.delay.queue";

            //var exchangetest = "exchange.business.test";
            //var routetest = "businessRoutingkey";
            //var queuetest = "queue.business.test";

            //创建连接
            using (var connection = RabbitMQHelper.GetConnection())
            {
                //创建通道
                using (var channel = connection.CreateModel())
                {
                    //指定x-delayed-message 类型的交换机，并且添加x-delayed-type属性
                    channel.ExchangeDeclare(exchangePlugin, "x-delayed-message", true, false, arguments: 
                                new Dictionary<string, object>()
                                {
                                    {"x-delayed-type","direct" }
                                }
                     );
                    channel.QueueDeclare(queuedlxPlugin, true, false, false, null);
                    channel.QueueBind(queuedlxPlugin, exchangePlugin, routedlxPlugin);

                    var properties = channel.CreateBasicProperties();
                    Dictionary<string, object> headers = new Dictionary<string, object>()
                    {
                        {"x-delay","10000" }
                    };
                    properties.Persistent = true;
                    properties.Headers = headers;

                    Console.WriteLine("生产者开始发送消息");
                    for(int i = 0; i < 10; i++)
                    {
                        //发布消息
                        var message = string.Format("{0}:message",i);
                        Console.WriteLine("Send Direct {0} message", i);
                        var body = Encoding.UTF8.GetBytes(message);
                        //发送一条延时10秒的消息
                        channel.BasicPublish(exchangePlugin, routedlxPlugin, properties, body);

                    }
                }
            }
            Console.ReadLine();
        }
    }
}
