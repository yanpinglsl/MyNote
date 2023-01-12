using RabbitMQ.Client;
using RabbitMQ_Consumer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQ_Provider.DCL
{
    public class DCLSend
    {

        public static void SendMessage()
        {
            var exchangeA = "changeA";
            var routeA = "routeA";
            var queueA = "queueA";

            var exchangeD = "changeD";
            var routeD = "routeD";
            var queueD = "queueD";

            using (var connection = RabbitMQHelper.GetConnection())
            {
                using(var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchangeD, type: "fanout", durable: true, autoDelete: false);
                    channel.QueueDeclare(queueD, durable: true, exclusive: false, autoDelete: false);
                    channel.QueueBind(queueD, exchangeD, routeD);

                    channel.ExchangeDeclare(exchangeA, type: "fanout", durable: true, autoDelete: false);
                    channel.QueueDeclare(queueA, durable: true, exclusive: false, autoDelete: false, arguments: 
                                        new Dictionary<string, object> {
                                             { "x-dead-letter-exchange",exchangeD}, //设置当前队列的DLX
                                             { "x-dead-letter-routing-key",routeD}, //设置DLX的路由key，DLX会根据该值去找到死信消息存放的队列
                                             { "x-message-ttl",10000} //设置消息的存活时间，即过期时间
                                         });
                    channel.QueueBind(queueA, exchangeA, routeA);


                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;
                    //发布消息
                    channel.BasicPublish(exchange: exchangeA,
                                         routingKey: routeA,
                                         basicProperties: properties,
                                         body: Encoding.UTF8.GetBytes("message"));
                }
            }
            
        } 
    }
}
