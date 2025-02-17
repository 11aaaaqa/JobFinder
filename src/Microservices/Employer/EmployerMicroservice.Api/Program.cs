using EmployerMicroservice.Api.Database;
using EmployerMicroservice.Api.Kafka.Consumers;
using EmployerMicroservice.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(
    x => x.UseNpgsql(builder.Configuration["Database:ConnectionString"]));

builder.Services.AddTransient<IEmployerRepository, EmployerRepository>();

builder.Services.AddHostedService<UserRegisteredKafkaConsumer>();
builder.Services.AddHostedService<CompanyAddedKafkaConsumer>();
builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();