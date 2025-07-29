using Microsoft.EntityFrameworkCore;
using NotificationMicroservice.Api.Database;
using NotificationMicroservice.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(x => 
    x.UseNpgsql(builder.Configuration["Database:ConnectionString"]));

builder.Services.AddTransient<INotificationService, NotificationService>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();
