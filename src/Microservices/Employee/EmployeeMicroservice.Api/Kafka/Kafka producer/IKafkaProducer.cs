using Confluent.Kafka;

namespace EmployeeMicroservice.Api.Kafka.Kafka_producer
{
    public interface IKafkaProducer
    {
        Task ProduceAsync(string topic, Message<Null, string> message);
    }
}
