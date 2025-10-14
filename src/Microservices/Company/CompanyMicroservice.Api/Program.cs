using CompanyMicroservice.Api.Database;
using CompanyMicroservice.Api.Kafka.Consumers;
using CompanyMicroservice.Api.Kafka.Producer;
using CompanyMicroservice.Api.Services;
using CompanyMicroservice.Api.Services.Pagination;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(
    x => x.UseNpgsql(builder.Configuration["Database:ConnectionString"]));

builder.Services.AddTransient<ICheckForNextPageExisting, CheckForNextPageExisting>();
builder.Services.AddTransient<ICompanyEmployerRepository, CompanyEmployerRepository>();
builder.Services.AddTransient<ICompanyRepository, CompanyRepository>();
builder.Services.AddTransient<IKafkaProducer, KafkaProducer>();

builder.Services.AddHostedService<CompanyRatingUpdatedKafkaConsumer>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();
