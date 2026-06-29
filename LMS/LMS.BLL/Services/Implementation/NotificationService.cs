using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.BLL.Services.Interfaces;
using LMS.BLL.ViewModels;
using LMS.DAL.Data;
using LMS.Domain.Enums;
using LMS.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LMS.BLL.Services.Implementation
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRealTimeNotificationService _realTimeService;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationService(
            ApplicationDbContext context, 
            IRealTimeNotificationService realTimeService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _realTimeService = realTimeService;
            _userManager = userManager;
        }

        public async Task<NotificationDto> CreateAndSendToUserAsync(string userId, string title, string message, NotificationType type)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            var dto = MapToDto(notification);
            await _realTimeService.SendToUserAsync(userId, dto);

            return dto;
        }

        public async Task<NotificationDto> CreateAndSendToRoleAsync(string roleName, string title, string message, NotificationType type)
        {
            // We still need to save to DB for users in this role so they see it on refresh.
            var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
            
            var notifications = new List<Notification>();
            foreach (var user in usersInRole)
            {
                notifications.Add(new Notification
                {
                    UserId = user.Id,
                    Title = title,
                    Message = message,
                    Type = type,
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                });
            }

            if (notifications.Any())
            {
                _context.Notifications.AddRange(notifications);
                await _context.SaveChangesAsync();
            }

            var dto = new NotificationDto
            {
                Title = title,
                Message = message,
                Type = type,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };
            
            // Broadcast via SignalR group
            await _realTimeService.SendToRoleAsync(roleName, dto);

            return dto;
        }

        public async Task MarkAsReadAsync(int notificationId, string userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            if (notification != null && !notification.IsRead)
            {
                notification.IsRead = true;
                _context.Notifications.Update(notification);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(string userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(50) // Limit to recent 50
                .Select(n => MapToDto(n))
                .ToListAsync();

            return notifications;
        }

        private static NotificationDto MapToDto(Notification notification)
        {
            return new NotificationDto
            {
                Id = notification.Id,
                Title = notification.Title,
                Message = notification.Message,
                Type = notification.Type,
                CreatedAt = notification.CreatedAt,
                IsRead = notification.IsRead
            };
        }
    }
}
