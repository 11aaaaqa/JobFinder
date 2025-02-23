using System.Text.Json;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using EmployerMicroservice.Api.Constants;
using EmployerMicroservice.Api.Database;
using EmployerMicroservice.Api.Kafka.Consumer_Models;
using Microsoft.EntityFrameworkCore;

namespace EmployerMicroservice.Api.Kafka.Consumers
{
    public class EmployerJoinedCompanyKafkaConsumer(IConfiguration configuration, IServiceScopeFactory scopeFactory) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            const string topicName = "employer-joined-company-topic";
            var config = new ConsumerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"],
                AutoOffsetReset = AutoOffsetReset.Earliest,
                GroupId = KafkaConstants.GroupId,
                AllowAutoCreateTopics = true
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
                            new TopicSpecification{Name = topicName, NumPartitions = 1, ReplicationFactor = 1}
                        });
                    }
                    catch (Exception ex)
                    {
                        if(!ex.Message.ToLower().Contains("already exists"))
                            throw;
                    }
                    continue;
                }

                var model = JsonSerializer.Deserialize<EmployerJoinedCompanyModel>(consumeResult.Message.Value);
                var employer = await context.Employers.SingleAsync(x => x.Id == model.EmployerId, CancellationToken.None);
                employer.CompanyId = model.CompanyId;
                await context.SaveChangesAsync(CancellationToken.None);
            }

            consumer.Close();
        }
    }
}
