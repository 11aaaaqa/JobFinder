using Confluent.Kafka;

namespace ResumeMicroservice.Api.Kafka.Producing
{
    public interface IKafkaProducer
    {
        Task ProduceAsync(string topic, Message<Null, string> message);
    }
}
