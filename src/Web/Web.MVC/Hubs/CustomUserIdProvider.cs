using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Web.MVC.Hubs
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.User.FindFirst(ClaimTypes.Email)?.Value;
        }
    }
}
