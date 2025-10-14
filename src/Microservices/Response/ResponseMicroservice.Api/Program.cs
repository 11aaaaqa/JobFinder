using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using ResponseMicroservice.Api.Database;
using ResponseMicroservice.Api.Kafka.Consumers;
using ResponseMicroservice.Api.Services.Interview_invitation_services;
using ResponseMicroservice.Api.Services.Pagination;
using ResponseMicroservice.Api.Services.Vacancy_response_services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(
    x => x.UseNpgsql(builder.Configuration["Database:ConnectionString"]));

builder.Services.AddHostedService<ResumeDeletedKafkaConsumer>();

builder.Services.AddTransient<ICheckForNextPageExistingService, CheckForNextPageExistingService>();

builder.Services.AddTransient<IVacancyResponseService, VacancyResponseService>();
builder.Services.AddTransient<IInterviewInvitationService, InterviewInvitationService>();

builder.Services.AddHangfire(x => x.UsePostgreSqlStorage(y =>
    y.UseNpgsqlConnection(builder.Configuration["Database:HangfireConnectionString"])));
builder.Services.AddHangfireServer();
builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();
