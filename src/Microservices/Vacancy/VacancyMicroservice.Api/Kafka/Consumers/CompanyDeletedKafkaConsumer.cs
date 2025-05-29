using System.Text.Json;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.EntityFrameworkCore;
using VacancyMicroservice.Api.Constants;
using VacancyMicroservice.Api.Database;
using VacancyMicroservice.Api.Kafka.Consumer_models;

namespace VacancyMicroservice.Api.Kafka.Consumers
{
    public class CompanyDeletedKafkaConsumer(IConfiguration configuration, IServiceScopeFactory scopeFactory) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            const string topicName = "company-deleted-topic";
            var config = new ConsumerConfig
            {
                GroupId = KafkaConstants.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                BootstrapServers = configuration["Kafka:BootstrapServers"]
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

                var consumeResult = consumer.Consume(stoppingToken);
                var model = JsonSerializer.Deserialize<CompanyDeletedConsumerModel>(consumeResult.Message.Value);

                var companyVacancies = await context.Vacancies.Where(x => x.CompanyId == model.CompanyId)
                    .ToListAsync(CancellationToken.None);

                context.Vacancies.RemoveRange(companyVacancies);
                await context.SaveChangesAsync(CancellationToken.None);
            }

            consumer.Close();
        }
    }
}
