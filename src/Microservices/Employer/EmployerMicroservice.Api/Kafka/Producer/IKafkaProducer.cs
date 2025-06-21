using Confluent.Kafka;

namespace EmployerMicroservice.Api.Kafka.Producer
{
    public interface IKafkaProducer
    {
        Task ProduceAsync(string topicName, Message<Null, string> message);
    }
}
