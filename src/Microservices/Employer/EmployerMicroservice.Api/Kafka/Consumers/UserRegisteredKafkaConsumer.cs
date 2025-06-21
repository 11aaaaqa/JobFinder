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
                        Surname = account.Surname,
                        AccountId = account.AccountId
                    }, CancellationToken.None);
                    await context.SaveChangesAsync(CancellationToken.None);
                }
            }
            consumer.Close();
        }
    }
}
