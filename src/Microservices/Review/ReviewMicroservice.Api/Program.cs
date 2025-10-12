using Microsoft.EntityFrameworkCore;
using ReviewMicroservice.Api.Database;
using ReviewMicroservice.Api.Kafka.Producer;
using ReviewMicroservice.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(
    x => x.UseNpgsql(builder.Configuration["Database:ConnectionString"]));

builder.Services.AddScoped<IKafkaProducer, KafkaProducer>();
builder.Services.AddTransient<IReviewRepository, ReviewRepository>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();
