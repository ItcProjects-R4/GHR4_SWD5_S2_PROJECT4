using LMS.BLL.Services.Interfaces;
using LMS.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LMS.PL.ViewComponents
{
    public class NotificationViewComponent : ViewComponent
    {
        private readonly INotificationService _notificationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationViewComponent(INotificationService notificationService, UserManager<ApplicationUser> userManager)
        {
            _notificationService = notificationService;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = _userManager.GetUserId(UserClaimsPrincipal);
            if (string.IsNullOrEmpty(userId))
            {
                return Content(""); // Return empty if not logged in
            }

            var notifications = await _notificationService.GetUserNotificationsAsync(userId);
            return View(notifications);
        }
    }
}
