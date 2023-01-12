using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_Consumer.Dead
{
    /// <summary>
    /// 什么是消息确认机制?
    /// MQ消息确认类似于数据库中用到的 commit 语句，用于告诉broker本条消息是被消费成功了还是失败了；
    /// 平时默认消息在被接收后就被自动确认了，需要在创建消费者时、设置 autoAck: false 即可使用手动确认模式；
    /// ====================================================================================
    /// 什么是死信队列?
    /// 死信队列是用于接收普通队列发生失败的消息，其原理与普通队列相同；
    /// > 失败消息如：被消费者拒绝的消息、TTL超时的消息、队列达到最大数量无法写入的消息；
    /// 死信队列创建方法：
    /// > 在创建普通队列时，在参数"x-dead-letter-exchange"中定义失败消息转发的目标交换机；
    /// > 再创建一个临时队列，订阅"x-dead-letter-exchange"中指定的交换机；
    /// > 此时的临时队列就能接收到普通队列失败的消息了；
    /// > 可在消息的 Properties.headers.x-death 属性中查询到消息投递源信息和消息被投递的次数；
    /// </summary>
    public class DeadExchange
    {
        private static string _exchangeNormal = "Exchange.Normal";  //定义一个用于接收 正常 消息的交换机
        private static string _exchangeRetry = "Exchange.Retry";    //定义一个用于接收 重试 消息的交换机
        private static string _exchangeFail = "Exchange.Fail";      //定义一个用于接收 失败 消息的交换机
        private static string _queueNormal = "Queue.Noraml";        //定义一个用于接收 正常 消息的队列
        private static string _queueRetry = "Queue.Retry";          //定义一个用于接收 重试 消息的队列
        private static string _queueFail = "Queue.Fail";            //定义一个用于接收 失败 消息的队列

        public static void TestDemo()
        {
            var connection = RabbitMQHelper.GetConnection();
            var channel = connection.CreateModel();

            //声明交换机
            channel.ExchangeDeclare(exchange: _exchangeNormal, type: "topic");
            channel.ExchangeDeclare(exchange: _exchangeRetry, type: "topic");
            channel.ExchangeDeclare(exchange: _exchangeFail, type: "topic");

            //定义队列参数
            var queueNormalArgs = new Dictionary<string, object>();
            {
                queueNormalArgs.Add("x-dead-letter-exchange", _exchangeFail);   //指定死信交换机，用于将 Noraml 队列中失败的消息投递给 Fail 交换机
            }
            var queueRetryArgs = new Dictionary<string, object>();
            {
                queueRetryArgs.Add("x-dead-letter-exchange", _exchangeNormal);  //指定死信交换机，用于将 Retry 队列中超时的消息投递给 Noraml 交换机
                queueRetryArgs.Add("x-message-ttl", 6000);                      //定义 queueRetry 的消息最大停留时间 (原理是：等消息超时后由 broker 自动投递给当前绑定的死信交换机)                                                                             //定义最大停留时间为防止一些 待重新投递 的消息、没有定义重试时间而导致内存溢出
            }
            var queueFailArgs = new Dictionary<string, object>();
            {
                //暂无
            }

            //声明队列
            channel.QueueDeclare(queue: _queueNormal, durable: true, exclusive: false, autoDelete: false, arguments: queueNormalArgs);
            channel.QueueDeclare(queue: _queueRetry, durable: true, exclusive: false, autoDelete: false, arguments: queueRetryArgs);
            channel.QueueDeclare(queue: _queueFail, durable: true, exclusive: false, autoDelete: false, arguments: queueFailArgs);

            //为队列绑定交换机
            channel.QueueBind(queue: _queueNormal, exchange: _exchangeNormal, routingKey: "#");
            channel.QueueBind(queue: _queueRetry, exchange: _exchangeRetry, routingKey: "#");
            channel.QueueBind(queue: _queueFail, exchange: _exchangeFail, routingKey: "#");

            #region 创建一个普通消息消费者
            {
                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (sender, e) =>
                {
                    var _sender = (EventingBasicConsumer)sender;            //消息传送者
                    var _channel = _sender.Model;                           //消息传送通道
                    var _message = (BasicDeliverEventArgs)e;                //消息传送参数
                    var _headers = _message.BasicProperties.Headers;        //消息头
                    var _content = Encoding.UTF8.GetString(_message.Body.ToArray());  //消息内容
                    var _death = default(Dictionary<string, object>);       //死信参数

                    if (_headers != null && _headers.ContainsKey("x-death"))
                        _death = (Dictionary<string, object>)(_headers["x-death"] as List<object>)[0];

                    try
                    #region 消息处理
                    {
                        Console.WriteLine();
                        Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")}\t(1.0)消息接收：\r\n\t[deliveryTag={_message.DeliveryTag}]\r\n\t[consumerID={_message.ConsumerTag}]\r\n\t[exchange={_message.Exchange}]\r\n\t[routingKey={_message.RoutingKey}]\r\n\t[content={_content}]");

                        // throw new Exception("模拟消息处理失败效果。");

                        //处理成功时
                        Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")}\t(1.1)处理成功：\r\n\t[deliveryTag={_message.DeliveryTag}]");

                        //消息确认 (销毁当前消息)
                        _channel.BasicAck(deliveryTag: _message.DeliveryTag, multiple: false);
                    }
                    #endregion
                    catch (Exception ex)
                    #region 消息处理失败时
                    {
                        var retryCount = (long)(_death?["count"] ?? default(long)); //查询当前消息被重新投递的次数 (首次则为0)

                        Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")}\t(1.2)处理失败：\r\n\t[deliveryTag={_message.DeliveryTag}]\r\n\t[retryCount={retryCount}]");

                        if (retryCount >= 2)
                        #region 投递第3次还没消费成功时，就转发给 exchangeFail 交换机
                        {
                            //消息拒绝（投递给死信交换机，也就是上边定义的 ("x-dead-letter-exchange", _exchangeFail)）
                            _channel.BasicNack(deliveryTag: _message.DeliveryTag, multiple: false, requeue: false);
                        }
                        #endregion
                        else
                        #region 否则转发给 exchangeRetry 交换机
                        {
                            var interval = (retryCount + 1) * 10; //定义下一次投递的间隔时间 (单位：秒)
                                                                  //如：首次重试间隔10秒、第二次间隔20秒、第三次间隔30秒

                            //定义下一次投递的间隔时间 (单位：毫秒)
                            _message.BasicProperties.Expiration = (interval * 1000).ToString();

                            //将消息投递给 _exchangeRetry (会自动增加 death 次数)
                            _channel.BasicPublish(exchange: _exchangeRetry, routingKey: _message.RoutingKey, basicProperties: _message.BasicProperties, body: _message.Body);

                            //消息确认 (销毁当前消息)
                            _channel.BasicAck(deliveryTag: _message.DeliveryTag, multiple: false);
                        }
                        #endregion
                    }
                    #endregion
                };
                channel.BasicConsume(queue: _queueNormal, autoAck: false, consumer: consumer);
            }
            #endregion

            #region 创建一个失败消息消费者
            {
                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (sender, e) =>
                {
                    var _message = (BasicDeliverEventArgs)e;                //消息传送参数
                    var _content = Encoding.UTF8.GetString(_message.Body.ToArray());  //消息内容

                    Console.WriteLine();
                    Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")}\t(2.0)发现失败消息：\r\n\t[deliveryTag={_message.DeliveryTag}]\r\n\t[consumerID={_message.ConsumerTag}]\r\n\t[exchange={_message.Exchange}]\r\n\t[routingKey={_message.RoutingKey}]\r\n\t[content={_content}]");
                };

                channel.BasicConsume(queue: _queueFail, autoAck: true, consumer: consumer);
            }
            #endregion

            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")}\t正在运行中...");

            var cmd = default(string);
            while ((cmd = Console.ReadLine()) != "close")
            #region 模拟正常消息发布
            {
                var msgProperties = channel.CreateBasicProperties();
                var msgContent = $"消息内容_{DateTime.Now.ToString("HH:mm:ss.fff")}_{cmd}";

                channel.BasicPublish(exchange: _exchangeNormal, routingKey: "亚洲.中国.经济", basicProperties: msgProperties, body: Encoding.UTF8.GetBytes(msgContent));

                Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")}\t发送成功：{msgContent}");
                Console.WriteLine();
            }
            #endregion

            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")}\t正在关闭...");

            channel.ExchangeDelete(_exchangeNormal);
            channel.ExchangeDelete(_exchangeRetry);
            channel.ExchangeDelete(_exchangeFail);
            channel.QueueDelete(_queueNormal);
            channel.QueueDelete(_queueRetry);
            channel.QueueDelete(_queueFail);
            //channel.Abort();
            channel.Close(200, "Goodbye!");
            channel.Dispose();
            connection.Close(200, "Goodbye!");
            connection.Dispose();

            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")}\t运行结束。");
            Console.ReadKey();
        }
    }
}
