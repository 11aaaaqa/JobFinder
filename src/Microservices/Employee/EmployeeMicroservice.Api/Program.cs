using EmployeeMicroservice.Api.Database;
using EmployeeMicroservice.Api.Kafka;
using EmployeeMicroservice.Api.Kafka.Kafka_producer;
using EmployeeMicroservice.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(x =>
    x.UseNpgsql(builder.Configuration["Database:ConnectionString"]));

builder.Services.AddScoped<IKafkaProducer, KafkaProducer>();
builder.Services.AddTransient<IEmployeeRepository, EmployeeRepository>();

builder.Services.AddHostedService<UserRegisteredKafkaConsumer>();
builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();
