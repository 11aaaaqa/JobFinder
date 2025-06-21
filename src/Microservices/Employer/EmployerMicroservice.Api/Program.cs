using EmployerMicroservice.Api.Database;
using EmployerMicroservice.Api.Kafka.Consumers;
using EmployerMicroservice.Api.Kafka.Producer;
using EmployerMicroservice.Api.Services;
using EmployerMicroservice.Api.Services.Company_permissions_services;
using EmployerMicroservice.Api.Services.Pagination;
using EmployerMicroservice.Api.Services.Searching_services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(
    x => x.UseNpgsql(builder.Configuration["Database:ConnectionString"]));

builder.Services.AddTransient<IEmployerPermissionsService, EmployerPermissionsService>();
builder.Services.AddTransient<ISearchingService, SearchingService> ();
builder.Services.AddTransient<IPaginationService, PaginationService>();
builder.Services.AddTransient<IEmployerRepository, EmployerRepository>();

builder.Services.AddScoped<IKafkaProducer, KafkaProducer>();

builder.Services.AddHostedService<CompanyDeletedKafkaConsumer>();
builder.Services.AddHostedService<UserRegisteredKafkaConsumer>();
builder.Services.AddHostedService<CompanyAddedKafkaConsumer>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();