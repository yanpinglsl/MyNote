using Confluent.Kafka.Admin;
using Confluent.Kafka;

namespace Kafka_Producer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string topic = "my.topic.range.sticky2";
            string server = "192.168.1.102:9192";
            //主题不存在时需要先创建主题
            //如果主题已存在，则注释改行即可
            //await KafkaHelper.Instance.CreateTopic(server, topic, 7);
            await PartitionPolicyDemo.RoundRobin(server, topic);
            Console.WriteLine($"当前主题的分区数为：{KafkaHelper.Instance.GetPartitionCount(server, topic)}");
            Console.ReadLine();
        }
        public static async Task CreateTopic()
        {
            var config = new AdminClientConfig
            {
                BootstrapServers = "192.168.1.102:9292"
            };

            using var adminClient = new AdminClientBuilder(config).Build();

            var topicName = "my.topic";
            var partitions = 2;

            var topicSpecification = new TopicSpecification
            {
                Name = topicName,
                NumPartitions = 2
            };

            await adminClient.CreateTopicsAsync(new[] { topicSpecification });

            Console.WriteLine($"主题 [{topicName}] 分区数量[{partitions}] 创建成功");
        }
    }
}
