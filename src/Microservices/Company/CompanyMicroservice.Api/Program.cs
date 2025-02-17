using CompanyMicroservice.Api.Database;
using CompanyMicroservice.Api.Kafka.Producer;
using CompanyMicroservice.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(
    x => x.UseNpgsql(builder.Configuration["Database:ConnectionString"]));

builder.Services.AddTransient<ICompanyRepository, CompanyRepository>();
builder.Services.AddTransient<IKafkaProducer, KafkaProducer>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();
