using Microsoft.EntityFrameworkCore;
using ResponseMicroservice.Api.Database;
using ResponseMicroservice.Api.Services.Interview_invitation_services;
using ResponseMicroservice.Api.Services.Vacancy_response_services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(
    x => x.UseNpgsql(builder.Configuration["Database:ConnectionString"]));

builder.Services.AddTransient<IVacancyResponseService, VacancyResponseService>();
builder.Services.AddTransient<IInterviewInvitationService, InterviewInvitationService>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();
