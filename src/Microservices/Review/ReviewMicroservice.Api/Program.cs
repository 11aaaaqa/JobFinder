using Microsoft.EntityFrameworkCore;
using ReviewMicroservice.Api.Database;
using ReviewMicroservice.Api.Kafka.Producer;
using ReviewMicroservice.Api.Services;
using ReviewMicroservice.Api.Services.Pagination;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(
    x => x.UseNpgsql(builder.Configuration["Database:ConnectionString"]));

builder.Services.AddScoped<IKafkaProducer, KafkaProducer>();
builder.Services.AddTransient<IReviewRepository, ReviewRepository>();
builder.Services.AddTransient<IPaginationService, PaginationService>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();
