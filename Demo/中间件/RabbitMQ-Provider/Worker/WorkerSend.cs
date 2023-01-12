using RabbitMQ.Client;
using RabbitMQ_Consumer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQ_Provider.Worker
{
    public class WorkerSend
    {

        public static void SendMessage()
        {
            string queueName = "Worker_Queue";

            using (var connection = RabbitMQHelper.GetConnection())
            {
                using(var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queueName, false, false, false, null);
                    for (int i = 0; i < 30; i++)
                    {
                        string message = $"RabbitMQ Worker {i + 1} Message";
                        var body = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish("", queueName, null, body);
                        Console.WriteLine("send Task {0} message",i + 1);
                    }
                   
                }
            }
            
        } 
    }
}
