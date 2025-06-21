using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace EmployerMicroservice.Api.Kafka.Producer
{
    public class KafkaProducer(IConfiguration configuration) : IKafkaProducer
    {
        public async Task ProduceAsync(string topicName, Message<Null, string> message)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"],
                Acks = Acks.All
            };
            using var adminClient = new AdminClientBuilder(config).Build();
            var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
            bool topicExists = metadata.Topics.Exists(x => x.Topic == topicName);
            if (!topicExists)
            {
                await adminClient.CreateTopicsAsync(new List<TopicSpecification>
                {
                    new (){Name = topicName, NumPartitions = 1, ReplicationFactor = 1}
                });
            }

            using var producer = new ProducerBuilder<Null, string>(config).Build();

            await producer.ProduceAsync(topicName, message);
            producer.Flush(TimeSpan.FromSeconds(10));
        }
    }
}
