using Confluent.Kafka;

namespace EmployeeMicroservice.Api.Kafka.Kafka_producer
{
    public class KafkaProducer(IConfiguration configuration) : IKafkaProducer
    {
        public async Task ProduceAsync(string topic, Message<Null, string> message)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"],
                AllowAutoCreateTopics = true,
                Acks = Acks.All
            };

            using var producer = new ProducerBuilder<Null, string>(config).Build();

            await producer.ProduceAsync(topic, message);

            producer.Flush();
        }
    }
}
