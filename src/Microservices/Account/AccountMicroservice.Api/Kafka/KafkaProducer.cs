using Confluent.Kafka;

namespace AccountMicroservice.Api.Kafka
{
    public class KafkaProducer : IKafkaProducer
    {
        private readonly IConfiguration configuration;
        public KafkaProducer(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

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