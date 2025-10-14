using System.Text.Json;
using CompanyMicroservice.Api.Constants;
using CompanyMicroservice.Api.Database;
using CompanyMicroservice.Api.Kafka.Consumer_models;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.EntityFrameworkCore;

namespace CompanyMicroservice.Api.Kafka.Consumers
{
    public class CompanyRatingUpdatedKafkaConsumer(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            const string topicName = "company-rating-updated-topic";
            var config = new ConsumerConfig
            {
                GroupId = KafkaConstants.GroupId,
                BootstrapServers = configuration["Kafka:BootstrapServers"],
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var adminClient = new AdminClientBuilder(config).Build();
            var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
            bool isTopicExisted = metadata.Topics.Exists(x => x.Topic == topicName);
            if (!isTopicExisted)
            {
                try
                {
                    await adminClient.CreateTopicsAsync(new List<TopicSpecification>
                    {
                        new TopicSpecification { Name = topicName, NumPartitions = 1, ReplicationFactor = 1 }
                    });
                }
                catch (Exception exc)
                {
                    if (!exc.Message.Contains("already exists"))
                        throw;
                }
            }

            using var consumer = new ConsumerBuilder<Null, string>(config).Build();
            consumer.Subscribe(topicName);

            using var scope = serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            while (!stoppingToken.IsCancellationRequested)
            {
                var consumeResult = consumer.Consume(stoppingToken);
                var model = JsonSerializer.Deserialize<CompanyRatingUpdatedConsumerModel>(consumeResult.Message.Value);

                var company = await context.Companies.SingleAsync(x => x.Id == model.CompanyId, CancellationToken.None);
                company.Rating = model.NewCompanyRating;
                await context.SaveChangesAsync(CancellationToken.None);
            }

            consumer.Close();
        }
    }
}