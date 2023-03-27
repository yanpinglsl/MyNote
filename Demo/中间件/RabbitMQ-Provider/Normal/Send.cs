using RabbitMQ.Client;
using RabbitMQ_Consumer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQ_Provider.Noraml
{
    public class Send
    {

        public static void SendMessage()
        {
            string queueName = "normal";

            using (var connection = RabbitMQHelper.GetConnection())
            {
                // 创建信道
                using(var channel = connection.CreateModel())
                {
                    // 创建队列
                    /**
                    * 生成一个队列
                    * 1.队列名称
                    * 2.队列里面的消息是否持久化 默认消息存储在内存中
                    * 3.该队列是否只供一个消费者进行消费 是否进行共享 true 可以多个消费者消费
                    * 4.是否自动删除 最后一个消费者断开连接以后 该队列是否自动删除 true 自动删除
                    * 5.其他参数
                    */
                    channel.QueueDeclare(queue:queueName,durable: false, false, false, null);
                    // 没有绑定交换机，怎么找到路由队列的呢？
                    while (true)
                    {
                        string message = "Hello RabbitMQ Message";
                        var body = Encoding.UTF8.GetBytes(message);
                        // 发送消息到rabbitmq,使用rabbitmq中默认提供交换机路由,默认的路由Key和队列名称完全一致
                        /**
                        * 发送一个消息
                        * 1.发送到那个交换机
                        * 2.路由的 key 是哪个
                        * 3.其他的参数信息
                        * 4.发送消息的消息体
                        */
                        channel.BasicPublish(exchange: "", routingKey: queueName, null, body);
                        Thread.Sleep(1000);
                        Console.WriteLine("Send Normal message");
                    }
                   
                }
            }
            
        } 
    }
}
