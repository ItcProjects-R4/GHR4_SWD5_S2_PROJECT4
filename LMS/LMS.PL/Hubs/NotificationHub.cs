using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Threading.Tasks;
using System;

namespace LMS.PL.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var user = Context.User;
            if (user != null && user.Identity.IsAuthenticated)
            {
                // Add user to role-based groups
                var roles = user.FindAll(ClaimTypes.Role);
                foreach (var role in roles)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, role.Value);
                }
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var user = Context.User;
            if (user != null && user.Identity.IsAuthenticated)
            {
                var roles = user.FindAll(ClaimTypes.Role);
                foreach (var role in roles)
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, role.Value);
                }
            }
            
            await base.OnDisconnectedAsync(exception);
        }
    }
}
