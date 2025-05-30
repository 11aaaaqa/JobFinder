using System.Text.Json;
using BookmarkMicroservice.Api.Constants;
using BookmarkMicroservice.Api.Database;
using BookmarkMicroservice.Api.Kafka.Consumer_models;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.EntityFrameworkCore;

namespace BookmarkMicroservice.Api.Kafka.Consumers
{
    public class VacancyUpdatedKafkaConsumer(IConfiguration configuration, IServiceScopeFactory scopeFactory) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            const string topicName = "vacancy-updated-topic";

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
                var model = JsonSerializer.Deserialize<VacancyUpdatedKafkaModel>(consumeResult.Message.Value);

                var vacanciesToUpdate = await context.FavoriteVacancies.Where(x => x.VacancyId == model.VacancyId)
                        .ToListAsync(CancellationToken.None);

                foreach (var vacancy in vacanciesToUpdate)
                {
                    vacancy.Position = model.NewPosition; vacancy.SalaryFrom = model.NewSalaryFrom;
                    vacancy.SalaryTo = model.NewSalaryTo; vacancy.VacancyCity = model.NewVacancyCity;
                    vacancy.WorkExperience = model.NewWorkExperience;
                }

                await context.SaveChangesAsync(CancellationToken.None);
            }

            consumer.Close();
        }
    }
}
