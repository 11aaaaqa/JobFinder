using Microsoft.EntityFrameworkCore;
using ResumeMicroservice.Api.Database;
using ResumeMicroservice.Api.Services.Pagination;
using ResumeMicroservice.Api.Services.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(x => 
    x.UseNpgsql(builder.Configuration["Database:ConnectionString"]));

builder.Services.AddTransient<IResumeRepository, ResumeRepository>();
builder.Services.AddTransient<ICheckForNextPageExistingService, CheckForNextPageExistingService>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();
