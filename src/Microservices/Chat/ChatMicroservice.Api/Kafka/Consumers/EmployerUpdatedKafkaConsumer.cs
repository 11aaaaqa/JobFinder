using ChatMicroservice.Api.Constants;
using ChatMicroservice.Api.Database;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using System.Text.Json;
using ChatMicroservice.Api.Kafka.Consumer_models;
using Microsoft.EntityFrameworkCore;

namespace ChatMicroservice.Api.Kafka.Consumers
{
    public class EmployerUpdatedKafkaConsumer(IConfiguration configuration, IServiceScopeFactory scopeFactory) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            const string topicName = "employer-updated-topic";
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
                await adminClient.CreateTopicsAsync(new List<TopicSpecification> { new ()
                {
                    Name = topicName, NumPartitions = 1, ReplicationFactor = 1
                }});
            }

            using var consumer = new ConsumerBuilder<Null, string>(config).Build();
            consumer.Subscribe(topicName);

            using var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            while (!stoppingToken.IsCancellationRequested)
            {
                var consumeResult = consumer.Consume(stoppingToken);
                var model = JsonSerializer.Deserialize<EmployerUpdatedConsumerModel>(consumeResult.Message.Value);

                var chatsToUpdate = await context.Chats
                    .Where(x => x.EmployerId == model.EmployerId)
                    .ToListAsync(CancellationToken.None);

                foreach (var chat in chatsToUpdate)
                {
                    chat.EmployerFullName = model.NewName + " " + model.NewSurname;
                }

                await context.SaveChangesAsync(CancellationToken.None);
            }

            consumer.Close();
        }
    }
}
