using System.Collections.Generic;
using System.Threading.Tasks;
using LMS.BLL.ViewModels;
using LMS.Domain.Enums;

namespace LMS.BLL.Services.Interfaces
{
    public interface INotificationService
    {
        Task<NotificationDto> CreateAndSendToUserAsync(string userId, string title, string message, NotificationType type);
        Task<NotificationDto> CreateAndSendToRoleAsync(string roleName, string title, string message, NotificationType type);
        Task MarkAsReadAsync(int notificationId, string userId);
        Task MarkAllAsReadAsync(string userId);
        Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(string userId);
    }
}
