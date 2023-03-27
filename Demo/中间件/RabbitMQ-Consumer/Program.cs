
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ_Consumer.Confirm;
using RabbitMQ_Consumer.Dead;
using RabbitMQ_Consumer.Durable;
using RabbitMQ_Consumer.Exchange.Direct;
using RabbitMQ_Consumer.Exchange.Fanout;
using RabbitMQ_Consumer.Exchange.Topic;
using RabbitMQ_Consumer.Noraml;
using RabbitMQ_Consumer.Worker;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQ_Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            #region 测试普通队列模式
            //Receive.ReceiveMessage();
            #endregion

            #region 测试工作队列模式
            //WorkerReceive.ReceiveMessage();
            #endregion

            #region 扇形队列模式
            //FanoutConsumer.ConsumerMessage();
            #endregion

            #region 直接队列模式
            //DirectConsumer.ConsumerMessage();
            #endregion

            #region 测试模糊匹配队列模式
            //TopicConsumer.ConsumerMessage();
            #endregion

            #region 测试Nack回调消费
            //ConfirmConsumer.ReceiveMessage();
            #endregion

            #region 测试死信交换机
            //DeadExchange.TestDemo();
            #endregion

            #region 测试持久化消息消费
            DLXReceive.ReceiveMessage();
            // DurableConsumer.ReceiveMessage();
            #endregion

            Console.WriteLine("消费端已经启动");
            Console.ReadKey();
        }
    }
}
