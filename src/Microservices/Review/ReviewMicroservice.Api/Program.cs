using Microsoft.EntityFrameworkCore;
using ReviewMicroservice.Api.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(
    x => x.UseNpgsql(builder.Configuration["Database:ConnectionString"]));

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();
