using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_Provider.Confirm
{
    public class ConfirmDemo
    {
        public static void ConfirmModel()
        {
            ConnectionFactory factory = new ConnectionFactory { HostName = "192.168.200.101", UserName = "yp", Password = "yp", VirtualHost = "/" };
            using (IConnection conn = factory.CreateConnection())
            {
                using (IModel channel = conn.CreateModel())
                {
                    channel.ExchangeDeclare("my-exchange", ExchangeType.Direct);
                    channel.QueueDeclare("my-queue", true, false, false, null);
                    channel.QueueBind("my-queue", "my-exchange", "routing.user", null);
                    var properties = channel.CreateBasicProperties();
                    properties.DeliveryMode = 2;
                    byte[] message = Encoding.UTF8.GetBytes("发送消息！");
                    //方式1：普通confirm 
                    //NormalConfirm(channel, properties, message);
                    //方式2：批量confirm
                    //BatchConfirm(channel, properties, message);
                    //方式3：异步确认Ack
                    ListenerConfirm(channel, properties, message);
                }
            }
        }

        /// <summary>
        /// 方式1：普通confirm模式
        /// 每发送一条消息后，调用waitForConfirms()方法，等待服务器端confirm。实际上是一种串行confirm了。
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="properties"></param>
        /// <param name="message"></param>
        static void NormalConfirm(IModel channel, IBasicProperties properties, byte[] message)
        {
            //发布确认默认是没有开启的，如果要开启需要调用方法 confirmSelect，每当你要想使用发布确认，都需要在 channel 上调用该方法
            channel.ConfirmSelect();
            channel.BasicPublish("my-exchange", "routing.user", properties, message);
            if (!channel.WaitForConfirms())
            {
                Console.WriteLine("send message failed.");
            }
            Console.WriteLine("send message success.");
        }

        /// <summary>
        /// 方式2：批量confirm模式
        /// 每发送一批消息后，调用waitForConfirms()方法，等待服务器端confirm。
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="properties"></param>
        /// <param name="message"></param>
        static void BatchConfirm(IModel channel, IBasicProperties properties, byte[] message)
        {
            channel.ConfirmSelect();
            for (int i = 0; i < 10; i++)
            {
                channel.BasicPublish("my-exchange", "routing.user", properties, message);
            }
            if (!channel.WaitForConfirms())
            {
                Console.WriteLine("send message failed.");
            }
            Console.WriteLine("send message success.");
            channel.Close();
        }

        /// <summary>
        /// 使用异步回调方式监听消息是否正确送达
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="properties"></param>
        /// <param name="message"></param>
        static void ListenerConfirm(IModel channel, IBasicProperties properties, byte[] message)
        {
            channel.ConfirmSelect();//开启消息确认模式
            /*-------------Return机制：不可达的消息消息监听--------------*/
            //这个事件就是用来监听我们一些不可达的消息的内容的：比如某些情况下，如果我们在发送消息时，当前的exchange不存在或者指定的routingkey路由不到，这个时候如果要监听这种不可达的消息，就要使用 return
            EventHandler<BasicReturnEventArgs> evreturn = new EventHandler<BasicReturnEventArgs>((o, basic) =>
            {
                var rc = basic.ReplyCode; //消息失败的code
                var rt = basic.ReplyText; //描述返回原因的文本。
                var msg = Encoding.UTF8.GetString(basic.Body.Span); //失败消息的内容
                                                                    //在这里我们可能要对这条不可达消息做处理，比如是否重发这条不可达的消息呀，或者这条消息发送到其他的路由中等等
                                                                    //System.IO.File.AppendAllText("d:/return.txt", "调用了Return;ReplyCode:" + rc + ";ReplyText:" + rt + ";Body:" + msg);
                Console.WriteLine("send message failed,不可达的消息消息监听.");
            });
            channel.BasicReturn += evreturn;
            //消息发送成功的时候进入到这个事件：即RabbitMq服务器告诉生产者，我已经成功收到了消息
            EventHandler<BasicAckEventArgs> BasicAcks = new EventHandler<BasicAckEventArgs>((o, basic) =>
            {
                Console.WriteLine("send message success,Acks.");
            });
            //消息发送失败的时候进入到这个事件：即RabbitMq服务器告诉生产者，你发送的这条消息我没有成功的投递到Queue中，或者说我没有收到这条消息。
            EventHandler<BasicNackEventArgs> BasicNacks = new EventHandler<BasicNackEventArgs>((o, basic) =>
            {
                //MQ服务器出现了异常，可能会出现Nack的情况
                Console.WriteLine("send message fail,Nacks.");
            });
            channel.BasicAcks += BasicAcks;
            channel.BasicNacks += BasicNacks;

            //注意：如果需要EventHandler<BasicReturnEventArgs>事件监听不可达消息的时候，一定要将mandatory设为true
            //channel.BasicPublish("my-exchange",routingKey:"routing.abc", mandatory: true,properties, message);

            channel.BasicPublish("my-exchange423423", "routing.user", properties, message);
        }
    }
}
