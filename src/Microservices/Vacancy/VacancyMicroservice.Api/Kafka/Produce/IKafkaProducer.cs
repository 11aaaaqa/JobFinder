using Confluent.Kafka;

namespace VacancyMicroservice.Api.Kafka.Produce
{
    public interface IKafkaProducer
    {
        Task ProduceAsync(string topic, Message<Null, string> message);
    }
}
