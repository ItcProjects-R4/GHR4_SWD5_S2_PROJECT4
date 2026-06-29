using System.Threading.Tasks;
using LMS.BLL.ViewModels;

namespace LMS.BLL.Services.Interfaces
{
    public interface IRealTimeNotificationService
    {
        Task SendToUserAsync(string userId, NotificationDto notification);
        Task SendToRoleAsync(string roleName, NotificationDto notification);
    }
}
