using Confluent.Kafka;

namespace CompanyMicroservice.Api.Kafka.Producer
{
    public interface IKafkaProducer
    {
        Task ProduceAsync(string topic, Message<Null, string> message);
    }
}
