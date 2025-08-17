using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Web.MVC.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
    }
}
