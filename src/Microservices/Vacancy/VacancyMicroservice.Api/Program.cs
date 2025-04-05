using Microsoft.EntityFrameworkCore;
using VacancyMicroservice.Api.Database;
using VacancyMicroservice.Api.Services;
using VacancyMicroservice.Api.Services.Pagination;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(x => 
    x.UseNpgsql(builder.Configuration["Database:ConnectionString"]));

builder.Services.AddTransient<IPaginationService, PaginationService>();
builder.Services.AddTransient<IVacancyRepository, VacancyRepository>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();
