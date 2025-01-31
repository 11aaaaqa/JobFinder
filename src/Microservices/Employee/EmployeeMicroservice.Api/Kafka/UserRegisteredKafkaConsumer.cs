using System.Text.Json;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using EmployeeMicroservice.Api.Database;
using EmployeeMicroservice.Api.Kafka.Consumer_models;
using EmployeeMicroservice.Api.Models;
using EmployeeMicroservice.Api.Models.EmployeeSkills;
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
            var config = new ConsumerConfig
            {
                GroupId = "user-group",
                BootstrapServers = configuration["Kafka:BootstrapServers"],
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Null, string>(config).Build();
            consumer.Subscribe("user-registered-topic");

            using var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            while (!stoppingToken.IsCancellationRequested)
            {
                ConsumeResult<Null, string> consumeResult = new();
                try
                {
                    consumeResult = consumer.Consume(stoppingToken);
                }
                catch (ConsumeException ex)
                {
                    if (!ex.Message.ToLower().Contains("unknown topic"))
                        throw;
                    using var adminClient = new AdminClientBuilder(config).Build();
                    try
                    {
                        await adminClient.CreateTopicsAsync(new List<TopicSpecification>
                        {
                            new(){Name = "user-registered-topic", ReplicationFactor = 1, NumPartitions = 1}
                        });
                    }
                    catch (Exception exc)
                    {
                        if (!exc.Message.ToLower().Contains("already exists"))
                            throw;
                    }
                    continue;
                }
                var account = JsonSerializer.Deserialize<AccountConsumerModel>(consumeResult.Message.Value);
                if (account.AccountType == AccountTypeConstants.Employee)
                {
                    await context.Employees.AddAsync(new Employee
                    {
                        Email = account.Email,
                        Name = account.Name,
                        Id = Guid.NewGuid(),
                        Surname = account.Surname,
                        ForeignLanguages = new List<ForeignLanguage>(),
                        DesiredSalaryFrom = null,
                        DesiredSalaryTo = null,
                        DateOfBirth = DateOnly.MinValue,
                        Educations = new List<Education>(),
                        EmployeeExperience = new List<EmployeeExperience>(),
                        ReadyToMove = false,
                    }, CancellationToken.None);
                    await context.SaveChangesAsync(CancellationToken.None);
                }
            }
            consumer.Close();
        }
    }
}
