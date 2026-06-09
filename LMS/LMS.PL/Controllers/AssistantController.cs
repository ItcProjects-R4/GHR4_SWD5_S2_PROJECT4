//using LMS.Domain.Models;
//using LMS.PL.ViewModels;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using System.IO;
//using System.Threading.Tasks;

//namespace LMS.PL.Controllers
//{
//    public class AssistantController : Controller
//    {
//        private readonly UserManager<ApplicationUser> _userManager;
//        private readonly SignInManager<ApplicationUser> _signInManager;
//        private readonly IWebHostEnvironment _environment;

//        public AssistantController(
//            UserManager<ApplicationUser> userManager,
//            SignInManager<ApplicationUser> signInManager,
//            IWebHostEnvironment environment)
//        {
//            _userManager = userManager;
//            _signInManager = signInManager;
//            _environment = environment;
//        }


//        // 1. عرض الإعدادات
//        [HttpGet]
//        public async Task<IActionResult> Settings()
//        {
//            var user = await _userManager.GetUserAsync(User);

//            var model = new AssistantSettingsViewModel
//            {
//                FirstName = user?.FirstName ?? "Sarah",
//                LastName = user?.LastName ?? "Adams",
//                Email = user?.Email ?? "sarah.adams@lms-platform.com",
//                AvatarUrl = user?.AvatarUrl
//            };

//            return View(model);
//        }

//        // 2. تعديل الملف الشخصي
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> UpdateProfile(AssistantSettingsViewModel model)
//        {
//            if (!ModelState.IsValid)
//            {
//                return View("Settings", model);
//            }

//            var user = await _userManager.GetUserAsync(User);
//            if (user == null)
//            {
//                TempData["ErrorMessage"] = "You must be logged in.";
//                return RedirectToAction(nameof(Settings));
//            }

//            user.FirstName = model.FirstName;
//            user.LastName = model.LastName;

//            var result = await _userManager.UpdateAsync(user);
//            if (result.Succeeded)
//            {
//                TempData["SuccessMessage"] = "Profile updated successfully!";
//            }
//            else
//            {
//                TempData["ErrorMessage"] = "Failed to update profile.";
//            }

//            return RedirectToAction(nameof(Settings));
//        }

//        // 3. تغيير كلمة المرور
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> UpdatePassword(AssistantSettingsViewModel model)
//        {
//            if (string.IsNullOrEmpty(model.CurrentPassword) ||
//                string.IsNullOrEmpty(model.NewPassword))
//            {
//                TempData["ErrorMessage"] = "All password fields are required.";
//                return RedirectToAction(nameof(Settings));
//            }

//            var user = await _userManager.GetUserAsync(User);
//            if (user == null)
//            {
//                TempData["ErrorMessage"] = "You must be logged in.";
//                return RedirectToAction(nameof(Settings));
//            }

//            var result = await _userManager.ChangePasswordAsync(
//                user,
//                model.CurrentPassword,
//                model.NewPassword);

//            if (result.Succeeded)
//            {
//                await _signInManager.RefreshSignInAsync(user);
//                TempData["SuccessMessage"] = "Password updated successfully!";
//            }
//            else
//            {
//                TempData["ErrorMessage"] = "Failed to update password.";
//            }

//            return RedirectToAction(nameof(Settings));
//        }

//        // 4. تحديث الصورة
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> UpdateAvatar(IFormFile avatarFile)
//        {
//            var user = await _userManager.GetUserAsync(User);
//            if (user == null)
//            {
//                TempData["ErrorMessage"] = "You must be logged in.";
//                return RedirectToAction(nameof(Settings));
//            }

//            if (avatarFile != null && avatarFile.Length > 0)
//            {
//                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "avatars");
//                if (!Directory.Exists(uploadsFolder))
//                {
//                    Directory.CreateDirectory(uploadsFolder);
//                }

//                var uniqueFileName = Guid.NewGuid().ToString() + "_" + avatarFile.FileName;
//                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

//                using (var fileStream = new FileStream(filePath, FileMode.Create))
//                {
//                    await avatarFile.CopyToAsync(fileStream);
//                }

//                user.AvatarUrl = "/uploads/avatars/" + uniqueFileName;
//                await _userManager.UpdateAsync(user);

//                TempData["SuccessMessage"] = "Avatar updated successfully!";
//            }

//            return RedirectToAction(nameof(Settings));
//        }
//    }
//}

using LMS.BLL.Services.Interfaces;
using LMS.Domain.Models;
using LMS.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.PL.Controllers
{
    //[Authorize(Roles = "Assistant")]
    public class AssistantController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _environment;
        private readonly ISubmissionService _submissionService;

        public AssistantController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IWebHostEnvironment environment,
            ISubmissionService submissionService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _environment = environment;
            _submissionService = submissionService;
        }

        // ── Dashboard Action ──────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            var claims = user != null ? await _userManager.GetClaimsAsync(user) : null;
            var permissionsList = claims != null ? string.Join(", ", claims.Where(c => c.Type == "Permission").Select(c => c.Value)) : "None";

            var recentSubmissions = await _submissionService.GetRecentSubmissionsAsync(5);

            var recentViewModels = recentSubmissions.Select(s => new SubmissionListItemViewModel
            {
                Id = s.Id,
                StudentName = s.Student != null ? $"{s.Student.FirstName} {s.Student.LastName}".Trim() : "Unknown",
                StudentInitial = !string.IsNullOrEmpty(s.Student?.FirstName) ? s.Student.FirstName.Substring(0, 1).ToUpper() : "U",
                CourseTitle = s.Assignment?.Title ?? "Untitled Assignment",
                SubmittedAt = s.SubmittedAt,
                IsGraded = s.Status == LMS.Domain.Enums.SubmissionStatus.Graded,
                Grade = s.Grade ?? 0,
                Feedback = s.Comment ?? "No feedback provided."
            });

            var viewModel = new AssistantDashboardViewModel
            {
                PendingSubmissionsCount = await _submissionService.GetPendingSubmissionsCountAsync(),
                GradedTodayCount = await _submissionService.GetGradedTodayCountAsync(),
                ActivePermissions = string.IsNullOrEmpty(permissionsList) ? "None" : permissionsList,
                RecentSubmissions = recentViewModels
            };
            return View(viewModel);
        }

        // ── Submissions Action ───────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Submissions(string searchString, string statusFilter)
        {
            var submissions = await _submissionService.GetFilteredSubmissionsAsync(searchString, statusFilter);

            var viewModel = submissions.Select(s => new SubmissionListItemViewModel
            {
                Id = s.Id,
                StudentName = s.Student != null ? $"{s.Student.FirstName} {s.Student.LastName}".Trim() : "Unknown",
                StudentInitial = !string.IsNullOrEmpty(s.Student?.FirstName) ? s.Student.FirstName.Substring(0, 1).ToUpper() : "U",
                CourseTitle = s.Assignment?.Title ?? "Untitled Assignment",
                SubmittedAt = s.SubmittedAt,
                IsGraded = s.Status == LMS.Domain.Enums.SubmissionStatus.Graded,
                Grade = s.Grade ?? 0,
                Feedback = s.Comment ?? "No feedback provided."
            });

            return View(viewModel);
        }

        // ── Settings Actions ──────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Settings()
        {
            var user = await _userManager.GetUserAsync(User);

            var model = new AssistantSettingsViewModel
            {
                FirstName = user?.FirstName ?? "Sarah",
                LastName = user?.LastName ?? "Adams",
                Email = user?.Email ?? "sarah.adams@lms-platform.com",
                AvatarUrl = user?.AvatarUrl
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(AssistantSettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Settings", model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "You must be logged in.";
                return RedirectToAction(nameof(Settings));
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Profile updated successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update profile.";
            }

            return RedirectToAction(nameof(Settings));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword(AssistantSettingsViewModel model)
        {
            if (string.IsNullOrEmpty(model.CurrentPassword) ||
                string.IsNullOrEmpty(model.NewPassword))
            {
                TempData["ErrorMessage"] = "All password fields are required.";
                return RedirectToAction(nameof(Settings));
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "You must be logged in.";
                return RedirectToAction(nameof(Settings));
            }

            var result = await _userManager.ChangePasswordAsync(
                user,
                model.CurrentPassword,
                model.NewPassword);

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["SuccessMessage"] = "Password updated successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update password.";
            }

            return RedirectToAction(nameof(Settings));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAvatar(IFormFile avatarFile)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "You must be logged in.";
                return RedirectToAction(nameof(Settings));
            }

            if (avatarFile != null && avatarFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "avatars");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + avatarFile.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await avatarFile.CopyToAsync(fileStream);
                }

                user.AvatarUrl = "/uploads/avatars/" + uniqueFileName;
                await _userManager.UpdateAsync(user);

                TempData["SuccessMessage"] = "Avatar updated successfully!";
            }

            return RedirectToAction(nameof(Settings));
        }
    }
}