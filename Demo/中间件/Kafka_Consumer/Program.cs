namespace Kafka_Consumer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string topic = "my.topic.range.sticky2";
            string server = "192.168.1.102:9192";
            PartitionAssignmentStrategyDemo.Run(server,topic);
        }
    }
}
