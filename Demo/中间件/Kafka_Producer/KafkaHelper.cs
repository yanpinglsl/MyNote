using Confluent.Kafka.Admin;
using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kafka_Producer
{
    public class KafkaHelper
    {
        private static KafkaHelper instance = null;
        private static readonly object padlock = new object();

        KafkaHelper() { }
        public static KafkaHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new KafkaHelper();
                        }
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// 创建拥有指定分区的主题
        /// </summary>
        /// <param name="server"></param>
        /// <param name="topic"></param>
        /// <param name="partitions"></param>
        /// <returns></returns>

        public async Task CreateTopic(string server, string topic, int partitions)
        {
            var config = new AdminClientConfig
            {
                BootstrapServers = server
            };

            using var adminClient = new AdminClientBuilder(config).Build();


            //await RemoveExistTapic(server, topic);
            var topicSpecification = new TopicSpecification
            {
                Name = topic,
                NumPartitions = partitions
            };
            await adminClient.CreateTopicsAsync(new[] { topicSpecification });

            Console.WriteLine($"主题 [{topic}] 分区数量[{partitions}] 创建成功");
        }

        /// <summary>
        /// 删除后重新创建Topic还是会报错（需要配置kafka？？？）
        /// 实际环境中则需要考虑while循环的效率问题
        /// 如果您在删除主题后立即尝试创建相同的主题，可能会遇到错误。
        /// 这是因为在删除主题后，Kafka需要一些时间来进行清理和同步操作，以确保主题完全被删除。
        /// 在此期间，如果尝试立即创建同名的主题，可能会导致冲突和错误。
        /// 为了解决这个问题，您可以在删除主题后等待一段时间，以确保主题已完全删除
        /// </summary>
        /// <param name="server"></param>
        /// <param name="topic"></param>
        /// <returns></returns>
        public async Task RemoveExistTapic(string server, string topic)
        {
            // Kafka集群的连接配置
            var config = new AdminClientConfig
            {
                BootstrapServers = server
            };

            using (var adminClient = new AdminClientBuilder(config).Build())
            {
                bool isRemove = false;
                bool isExcute = false;
                while (!isRemove)
                {
                    var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
                    var topicMetadata = metadata.Topics.FirstOrDefault(topicMetadata => topicMetadata.Topic == topic);

                    if (topicMetadata != null)
                    {
                        if(isExcute == false)
                        {

                            // 调用DeleteTopicsAsync方法删除主题
                            await adminClient.DeleteTopicsAsync(new List<string> { topic });
                            isExcute = true;
                        }
                        else
                        {
                            await Task.Delay(TimeSpan.FromSeconds(10));

                        }
                    }
                    else
                    {
                        isRemove = true;
                    }
                }
            }
        }
        /// <summary>
        /// 获取主题的分区数
        /// </summary>
        /// <param name="topic"></param>
        public int GetPartitionCount(string server, string topic)
        {
            var config = new AdminClientConfig
            {
                BootstrapServers = server
            };

            using var adminClient = new AdminClientBuilder(config).Build();

            var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));

            var topicMetadata = metadata.Topics.FirstOrDefault(topicMetadata => topicMetadata.Topic == topic);

            return topicMetadata == null ? 0 : topicMetadata.Partitions.Count;
        }
    }
}
