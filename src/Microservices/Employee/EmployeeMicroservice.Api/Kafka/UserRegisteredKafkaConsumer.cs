using System.Text.Json;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using EmployeeMicroservice.Api.Database;
using EmployeeMicroservice.Api.Kafka.Consumer_models;
using EmployeeMicroservice.Api.Models;
using GeneralLibrary.Constants;

namespace EmployeeMicroservice.Api.Kafka
{
    public class UserRegisteredKafkaConsumer : BackgroundService
    {
        private readonly IConfiguration configuration;
        private readonly IServiceScopeFactory scopeFactory;
        public UserRegisteredKafkaConsumer(IConfiguration configuration, IServiceScopeFactory scopeFactory)
        {
            this.configuration = configuration;
            this.scopeFactory = scopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            const string topicName = "user-registered-topic";
            var config = new ConsumerConfig
            {
                GroupId = "employee-group",
                BootstrapServers = configuration["Kafka:BootstrapServers"],
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Null, string>(config).Build();
            consumer.Subscribe(topicName);

            using var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            while (!stoppingToken.IsCancellationRequested)
            {
                //ConsumeResult<Null, string> consumeResult = new();
                //try
                //{
                //    consumeResult = consumer.Consume(stoppingToken);
                //}
                //catch (ConsumeException ex)
                //{
                //    if (!ex.Message.ToLower().Contains("unknown topic"))
                //        throw;
                //    using var adminClient = new AdminClientBuilder(config).Build();
                //    try
                //    {
                //        await adminClient.CreateTopicsAsync(new List<TopicSpecification>
                //        {
                //            new(){Name = "user-registered-topic", ReplicationFactor = 1, NumPartitions = 1}
                //        });
                //    }
                //    catch (Exception exc)
                //    {
                //        if (!exc.Message.ToLower().Contains("already exists"))
                //            throw;
                //    }
                //    continue;
                //}

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

                var consumeResult = consumer.Consume(stoppingToken);
                var account = JsonSerializer.Deserialize<AccountConsumerModel>(consumeResult.Message.Value);
                if (account.AccountType == AccountTypeConstants.Employee)
                {
                    await context.Employees.AddAsync(new Employee
                    {
                        Email = account.Email,
                        Name = account.Name,
                        Id = Guid.NewGuid(),
                        Surname = account.Surname,
                        AccountId = account.AccountId,
                        City = null, DateOfBirth = null, PhoneNumber = null, Gender = null, Patronymic = null, Status = WorkStatusConstants.LookingForJob
                    }, CancellationToken.None);
                    await context.SaveChangesAsync(CancellationToken.None);
                }
            }
            consumer.Close();
        }
    }
}
