using BookmarkMicroservice.Api.Database;
using BookmarkMicroservice.Api.Kafka.Consumers;
using BookmarkMicroservice.Api.Services.Pagination;
using BookmarkMicroservice.Api.Services.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(x =>
    x.UseNpgsql(builder.Configuration["Database:ConnectionString"]));

builder.Services.AddHostedService<CompanyUpdatedKafkaConsumer>();
builder.Services.AddHostedService<VacancyDeletedKafkaConsumer>();
builder.Services.AddHostedService<VacancyUpdatedKafkaConsumer>();

builder.Services.AddTransient<IFavoriteVacancyRepository, FavoriteVacancyRepository>();
builder.Services.AddTransient<ICheckForNextPageExistingService, CheckForNextPageExistingService>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();
