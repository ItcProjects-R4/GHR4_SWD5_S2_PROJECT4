using System;
using LMS.Domain.Enums;

namespace LMS.BLL.ViewModels
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public NotificationType Type { get; set; }
    }
}
