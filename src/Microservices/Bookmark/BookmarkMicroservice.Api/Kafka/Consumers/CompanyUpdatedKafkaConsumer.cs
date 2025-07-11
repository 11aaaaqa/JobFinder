﻿using BookmarkMicroservice.Api.Kafka.Consumer_models;
using Confluent.Kafka.Admin;
using Confluent.Kafka;
using System.Text.Json;
using BookmarkMicroservice.Api.Constants;
using BookmarkMicroservice.Api.Database;
using Microsoft.EntityFrameworkCore;

namespace BookmarkMicroservice.Api.Kafka.Consumers
{
    public class CompanyUpdatedKafkaConsumer(IConfiguration configuration, IServiceScopeFactory scopeFactory) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            const string topicName = "company-updated-topic";
            var config = new ConsumerConfig
            {
                GroupId = KafkaConstants.GroupId,
                BootstrapServers = configuration["Kafka:BootstrapServers"],
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var adminClient = new AdminClientBuilder(config).Build();
            var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
            bool topicExists = metadata.Topics.Exists(x => x.Topic == topicName);
            if (!topicExists)
            {
                try
                {
                    await adminClient.CreateTopicsAsync(new List<TopicSpecification> { new ()
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

            using var consumer = new ConsumerBuilder<Null, string>(config).Build();
            consumer.Subscribe(topicName);

            using var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            while (!stoppingToken.IsCancellationRequested)
            {
                var consumeResult = consumer.Consume(stoppingToken);
                var model = JsonSerializer.Deserialize<CompanyUpdatedKafkaModel>(consumeResult.Message.Value);

                var vacanciesToUpdate = await context.FavoriteVacancies
                    .Where(x => x.CompanyName == model.OldCompanyName).ToListAsync(CancellationToken.None);

                foreach (var vacancy in vacanciesToUpdate)
                {
                    vacancy.CompanyName = model.NewCompanyName;
                }

                await context.SaveChangesAsync(CancellationToken.None);
            }

            consumer.Close();
        }
    }
}

