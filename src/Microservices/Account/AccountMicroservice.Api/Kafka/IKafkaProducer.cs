using Confluent.Kafka;

namespace AccountMicroservice.Api.Kafka
{
    public interface IKafkaProducer
    {
        Task ProduceAsync(string topic, Message<Null, string> message);
    }
}