using System.Text.Json;
using ChatMicroservice.Api.Constants;
using ChatMicroservice.Api.Database;
using ChatMicroservice.Api.Kafka.Consumer_models;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.EntityFrameworkCore;

namespace ChatMicroservice.Api.Kafka.Consumers
{
    public class EmployeeUpdatedKafkaConsumer(IConfiguration configuration, IServiceScopeFactory scopeFactory) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            const string topicName = "employee-updated-topic";
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
                var model = JsonSerializer.Deserialize<EmployeeUpdatedConsumerModel>(consumeResult.Message.Value);

                var chatsToUpdate = await context.Chats
                    .Where(x => x.EmployeeId == model.EmployeeId)
                    .ToListAsync(CancellationToken.None);

                foreach (var chat in chatsToUpdate)
                {
                    chat.EmployeeFullName = model.NewName + " " + model.NewSurname;
                }

                await context.SaveChangesAsync(CancellationToken.None);
            }

            consumer.Close();
        }
    }
}
