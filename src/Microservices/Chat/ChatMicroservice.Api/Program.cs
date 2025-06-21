using ChatMicroservice.Api.Database;
using ChatMicroservice.Api.Kafka.Consumers;
using ChatMicroservice.Api.Services.Chat_services;
using ChatMicroservice.Api.Services.Message_services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(x
    => x.UseNpgsql(builder.Configuration["Database:ConnectionString"]));

builder.Services.AddTransient<IChatService, ChatService>();
builder.Services.AddTransient<IMessageService, MessageService>();

builder.Services.AddHostedService<EmployeeUpdatedKafkaConsumer>();
builder.Services.AddHostedService<EmployerUpdatedKafkaConsumer>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();
