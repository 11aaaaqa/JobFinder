using Confluent.Kafka;
using Confluent.Kafka.Admin;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VacancyMicroservice.Api.Constants;
using VacancyMicroservice.Api.Database;
using VacancyMicroservice.Api.Kafka.Consumer_models;

namespace VacancyMicroservice.Api.Kafka.Consumers
{
    public class CompanyUpdatedKafkaConsumer(IConfiguration configuration, IServiceScopeFactory scopeFactory) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            const string topicName = "company-updated-topic";
            var config = new ConsumerConfig
            {
                GroupId = KafkaConstants.GroupId,
                AllowAutoCreateTopics = true,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                BootstrapServers = configuration["Kafka:BootstrapServers"]
            };
            using var consumer = new ConsumerBuilder<Null, string>(config).Build();

            consumer.Subscribe(topicName);

            using var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            while (!stoppingToken.IsCancellationRequested)
            {
                ConsumeResult<Null, string> consumeResult = new();
                try
                {
                    consumeResult = consumer.Consume(stoppingToken);
                }
                catch (Exception exc)
                {
                    if (!exc.Message.ToLower().Contains("unknown topic"))
                        throw;
                    using var adminClient = new AdminClientBuilder(config).Build();

                    try
                    {
                        await adminClient.CreateTopicsAsync(new List<TopicSpecification>
                        {
                            new() {Name = topicName, NumPartitions = 1, ReplicationFactor = 1}
                        });
                    }
                    catch (Exception ex)
                    {
                        if (!ex.Message.ToLower().Contains("already exists"))
                            throw;
                    }
                    continue;
                }

                var model = JsonSerializer.Deserialize<CompanyUpdatedConsumerModel>(consumeResult.Message.Value);

                var vacancies = await context.Vacancies.Where(x => x.CompanyId == model.CompanyId).ToListAsync(CancellationToken.None);
                foreach (var vacancy in vacancies)
                {
                    vacancy.CompanyName = model.NewCompanyName;
                }

                await context.SaveChangesAsync(CancellationToken.None);
            }

            consumer.Close();
        }
    }
}
