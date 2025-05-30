using System.Text.Json;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using EmployerMicroservice.Api.Constants;
using EmployerMicroservice.Api.Database;
using EmployerMicroservice.Api.Kafka.Consumer_Models;
using Microsoft.EntityFrameworkCore;

namespace EmployerMicroservice.Api.Kafka.Consumers
{
    public class CompanyAddedKafkaConsumer(IConfiguration configuration, IServiceScopeFactory scopeFactory) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            const string topicName = "company-added-topic";
            var config = new ConsumerConfig
            {
                GroupId = KafkaConstants.GroupId,
                BootstrapServers = configuration["Kafka:BootstrapServers"],
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            using var consumer = new ConsumerBuilder<Null, string>(config).Build();

            consumer.Subscribe(topicName);

            using var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            while (!stoppingToken.IsCancellationRequested)
            {
                using var adminClient = new AdminClientBuilder(config).Build();
                var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
                bool topicExists = metadata.Topics.Exists(x => x.Topic == topicName);
                if (!topicExists)
                {
                    try
                    {
                        await adminClient.CreateTopicsAsync(new List<TopicSpecification> { new TopicSpecification
                        {
                            Name = topicName, NumPartitions = 1, ReplicationFactor = 1
                        }});
                    }
                    catch (Exception exc)
                    {
                        if (!exc.Message.ToLower().Contains("already exists"))
                            throw;
                    }
                }

                ConsumeResult<Null, string> consumeResult;
                try
                {
                    consumeResult = consumer.Consume(stoppingToken);
                }
                catch (Exception e)
                {
                    if (!e.Message.ToLower().Contains("unknown topic"))
                        throw;
                    try
                    {
                        await adminClient.CreateTopicsAsync(new List<TopicSpecification> { new TopicSpecification
                        {
                            Name = topicName, NumPartitions = 1, ReplicationFactor = 1
                        }});
                    }
                    catch (Exception exc)
                    {
                        if (!exc.Message.ToLower().Contains("already exists"))
                            throw;
                    }
                    continue;
                }
                var model = JsonSerializer.Deserialize<CompanyAddedConsumerModel>(consumeResult.Message.Value);
                var employer = await context.Employers.SingleAsync(x => x.Id == model.EmployerId,
                    cancellationToken: CancellationToken.None);

                employer.CompanyId = model.CompanyId;
                await context.SaveChangesAsync(CancellationToken.None);
            }

            consumer.Close();
        }
    }
}
