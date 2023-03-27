using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_Consumer
{
    public class DLXReceive
    {
        public static void ReceiveMessage()
        {
            var exchangeA = "changeA";
            var routeA = "routeA";
            var queueA = "queueA";

            var exchangeD = "changeD";
            var routeD = "routeD";
            var queueD = "queueD";

            var connection = RabbitMQHelper.GetConnection();
            {
                var channel = connection.CreateModel();
                {
                    channel.ExchangeDeclare(exchangeD, type: "fanout", durable: true, autoDelete: false);
                    channel.QueueDeclare(queueD, durable: true, exclusive: false, autoDelete: false);
                    channel.QueueBind(queueD, exchangeD, routeD);

                    channel.ExchangeDeclare(exchangeA, type: "fanout", durable: true, autoDelete: false);
                    channel.QueueDeclare(queueA, durable: true, exclusive: false, autoDelete: false, arguments:
                                        new Dictionary<string, object> {
                                             { "x-dead-letter-exchange",exchangeD}, //设置当前队列的DLX
                                             { "x-dead-letter-routing-key",routeD} //设置DLX的路由key，DLX会根据该值去找到死信消息存放的队列
                                            // { "x-message-ttl",10000} //设置消息的存活时间，即过期时间
                                             //{ "x-max-length", 5 }//队列最大长度为100，超出这个长度后接收的消息为dead message
                });
                    channel.QueueBind(queueA, exchangeA, routeA);

                                        //推送的消息如何进行消费的接口回调
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                        if (message.StartsWith("8:"))
                        {
                            channel.BasicReject(ea.DeliveryTag, requeue: false);//拒收消息
                            Console.WriteLine(" Reject Received => {0}", message);
                        }
                        else
                        {
                            channel.BasicAck(ea.DeliveryTag, multiple: false);
                            Console.WriteLine(" Normal Received => {0}", message);

                        }
                    };
                    channel.BasicConsume(queueA, false, consumer);
                }
            }

        }
    }
}
