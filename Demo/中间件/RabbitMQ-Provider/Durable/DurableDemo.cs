using RabbitMQ_Consumer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_Provider.Durable
{
    /// <summary>
    /// RabbitMQ消息持久化验证
    /// 1、发送消息必须有持久化标签
    /// 2、交换机必须是持久化
    /// 3、队列必须持久化
    /// 
    /// </summary>
    public class DurableDemo
    {
        public static void SendDurableMessage()
        {
            var connection = RabbitMQHelper.GetConnection();
            var channel = connection.CreateModel();
            //1、 创建持久化队列
            channel.QueueDeclare("durable_queue", true, false, false, null);
            //2、 创建持久化的交换机
            channel.ExchangeDeclare("durable_exchange", "fanout", false, false, null);
            channel.QueueBind("durable_queue", "durable_exchange", "", null);
            //3、消息持久化
            var properties = channel.CreateBasicProperties();
            //properties.Persistent = true; // 标记消息持久化
            properties.DeliveryMode = 2;
            for (int i = 0; i < 30; i++)
            {
                string message = $"Druable Message number is {i + 1}";
                byte[] body = Encoding.UTF8.GetBytes(message);
                // 发送消息
                channel.BasicPublish("durable_exchange", "",false, properties, body);
            }
        }
    }
}
