using Confluent.Kafka;

namespace ReviewMicroservice.Api.Kafka.Producer
{
    public interface IKafkaProducer
    {
        Task ProduceAsync(string topic, Message<Null,string> message);
    }
}
