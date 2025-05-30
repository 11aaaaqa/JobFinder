using System.Text.Json;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.EntityFrameworkCore;
using ResumeMicroservice.Api.Constants;
using ResumeMicroservice.Api.Database;
using ResumeMicroservice.Api.Kafka.Consumer_models;

namespace ResumeMicroservice.Api.Kafka.Consumers
{
    public class EmployeeStatusUpdatedKafkaConsumer(IConfiguration configuration,
        IServiceScopeFactory scopeFactory) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            const string topicName = "employee-status-updated-topic";
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

                var model = JsonSerializer.Deserialize<EmployeeStatusUpdatedConsumerModel>(consumeResult.Message.Value);
                var resumes = await context.Resumes.Where(x => x.EmployeeId == model.EmployeeId)
                    .ToListAsync(CancellationToken.None);
                foreach (var resume in resumes)
                {
                    resume.Status = model.Status;
                }

                await context.SaveChangesAsync(CancellationToken.None);
            }

            consumer.Close();
        }
    }
}
