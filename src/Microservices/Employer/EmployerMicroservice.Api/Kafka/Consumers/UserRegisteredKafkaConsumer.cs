using System.Text.Json;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using EmployerMicroservice.Api.Database;
using EmployerMicroservice.Api.Kafka.Consumer_Models;
using GeneralLibrary.Constants;

namespace EmployerMicroservice.Api.Kafka.Consumers
{
    public class UserRegisteredKafkaConsumer(IConfiguration configuration, IServiceScopeFactory scopeFactory) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            const string topicName = "user-registered-topic";
            var config = new ConsumerConfig
            {
                GroupId = "employer-group",
                BootstrapServers = configuration["Kafka:BootstrapServers"],
                AutoOffsetReset = AutoOffsetReset.Earliest
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
                            new (){Name = topicName, ReplicationFactor = 1, NumPartitions = 1}
                        });
                    }
                    catch (Exception ex)
                    {
                        if (!ex.Message.ToLower().Contains("already exists"))
                            throw;
                    }
                    continue;
                }

                var account = JsonSerializer.Deserialize<UserRegisteredConsumerModel>(consumeResult.Message.Value);
                if (account.AccountType == AccountTypeConstants.Employer)
                {
                    await context.Employers.AddAsync(new Models.Employer
                    {
                        Name = account.Name,
                        Email = account.Email,
                        CompanyPost = null,
                        CompanyId = null,
                        Id = Guid.NewGuid(),
                        Surname = account.Surname
                    }, CancellationToken.None);
                    await context.SaveChangesAsync(CancellationToken.None);
                }
            }
            consumer.Close();
        }
    }
}
