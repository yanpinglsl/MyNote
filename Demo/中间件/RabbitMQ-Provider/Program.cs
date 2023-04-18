using RabbitMQ.Client;
using RabbitMQ_Provider.Confirm;
using RabbitMQ_Provider.DCL;
using RabbitMQ_Provider.Delay;
using RabbitMQ_Provider.Durable;
using RabbitMQ_Provider.Exchange.Direct;
using RabbitMQ_Provider.Exchange.Fanout;
using RabbitMQ_Provider.Exchange.Topic;
using RabbitMQ_Provider.Noraml;
using RabbitMQ_Provider.Normal;
using RabbitMQ_Provider.Worker;
using System;

namespace RabbitMQ_Provider
{
    class Program
    {
        static void Main(string[] args)
        {
            //RabbitMQConnection.SendMessage();

            #region 测试普通队列模式
            //Send.SendMessage();
            #endregion

            #region 测试工作队列模式
            //WorkerSend.SendMessage();
            #endregion

            #region 测试扇形队列模式
            //FanoutSend.SendMessage();
            #endregion

            #region 测试匹配直接队列模式
            //DirectSend.SendMessage();
            #endregion

            #region 测试模糊匹配队列模式
            //TopicProvider.SendMessage();
            #endregion

            #region 测试消息确认机制
            // 确认方式
            //ConfirmDemo.ConfirmModel();
            // 事务方式
            //Transaction.TransactionMode();
            #endregion

            #region 测试持久化操作
            // DurableDemo.SendDurableMessage();
            #endregion

            #region 测试优先级队列
            // PriorityProvider.SendMessage();
            #endregion

            #region 测试死信队列
            //DCLSend.SendMessage();
            #endregion

            #region 测试延迟队列
            DelayProvider.SendMessage();
            //DelayPluginProvider.SendMessage();
            #endregion


            Console.WriteLine("=====================Provider================");
        }
    }
}
