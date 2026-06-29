using LMS.BLL.Services.Interfaces;
using LMS.BLL.ViewModels;
using LMS.PL.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace LMS.PL.Services
{
    public class RealTimeNotificationService : IRealTimeNotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public RealTimeNotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendToUserAsync(string userId, NotificationDto notification)
        {
            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", notification);
        }

        public async Task SendToRoleAsync(string roleName, NotificationDto notification)
        {
            await _hubContext.Clients.Group(roleName).SendAsync("ReceiveNotification", notification);
        }
    }
}
