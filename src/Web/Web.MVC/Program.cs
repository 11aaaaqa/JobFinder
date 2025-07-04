using System.Net;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using Web.MVC.Chat_services;
using Web.MVC.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAntiforgery(x => x.Cookie.Name = "af_token");
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
    };
});

builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

builder.Services.AddSignalR();
builder.Services.AddHttpClient();
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
else
{
    app.UseDeveloperExceptionPage();
}
app.UseRouting();

app.UseMiddleware<JwtTokenMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.UseStatusCodePages(context =>
{
    var response = context.HttpContext.Response;
    if (response.StatusCode == (int)HttpStatusCode.Unauthorized)
    {
        string returnUrl = context.HttpContext.Request.Path;
        if (context.HttpContext.Request.QueryString.HasValue)
            returnUrl += context.HttpContext.Request.QueryString;

        var encodedReturnUrl = HttpUtility.UrlEncode(returnUrl);
        response.Redirect($"/auth/login?returnUrl={encodedReturnUrl}");
    }

    if (response.StatusCode == (int)HttpStatusCode.Forbidden)
    {
        response.Redirect("/forbidden");
    }

    if (response.StatusCode == (int)HttpStatusCode.NotFound)
    {
        response.Redirect("/not-found");
    }

    if (response.StatusCode == (int)HttpStatusCode.InternalServerError)
    {
        response.Redirect("/server-error");
    }

    if (response.StatusCode == (int)HttpStatusCode.BadRequest)
    {
        response.Redirect("/bad-request");
    }

    return Task.CompletedTask;
});

app.MapStaticAssets();

app.MapHub<ChatHub>("/chat");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
