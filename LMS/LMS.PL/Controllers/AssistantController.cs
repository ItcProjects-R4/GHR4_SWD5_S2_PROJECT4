using LMS.BLL.Services.Interfaces;
using LMS.Domain.Models;
using LMS.Domain.ViewModels;
using LMS.Domain.ViewModels.Account;
using LMS.Domain.ViewModels.Assistant;
using LMS.Domain.ViewModels.Shared;
using LMS.PL.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.PL.Controllers
{
    [Authorize(Roles = "Assistant")]
    public class AssistantController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ISubmissionService _submissionService;
        private readonly IAccountService _accountService;
        private readonly IAssistantService _assistantService;

        public AssistantController(
       UserManager<ApplicationUser> userManager,
       SignInManager<ApplicationUser> signInManager,
       ISubmissionService submissionService,
       IAccountService accountService,
       ICloudinaryService cloudinaryService,
       IAssistantService assistantService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _submissionService = submissionService;
            _cloudinaryService = cloudinaryService;
            _accountService = accountService;
            _assistantService = assistantService;

        }

        // ── Dashboard Action ──────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var model = await _assistantService.GetDashboardAsync();

            var user = await _userManager.GetUserAsync(User);
            var claims = user != null ? await _userManager.GetClaimsAsync(user) : null;

            model.ActivePermissions = claims != null
                ? string.Join(", ", claims.Where(c => c.Type == "Permission").Select(c => c.Value))
                : "None";

            return View(model);
        }

        // ── Submissions Action ───────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Submissions(string searchString, string statusFilter)
        {
            var model = await _assistantService.GetSubmissionsAsync(searchString, statusFilter);

            return View(model);
        }
        // ── Settings Actions ──────────────────────────────────────


        [HttpGet]
       
        public async Task<IActionResult> Settings()
        {
            var userId = _userManager.GetUserId(User);
            var profileData = await _accountService.GetProfileSettingsAsync(userId);

            return View(profileData);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UpdateProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
               
                var userId = _userManager.GetUserId(User);
                var profileData = await _accountService.GetProfileSettingsAsync(userId);
                return View("Settings", profileData);
            }

            var currentUserId = _userManager.GetUserId(User);
            var result = await _accountService.UpdateProfileAsync(currentUserId, model);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByIdAsync(currentUserId);
                await _signInManager.RefreshSignInAsync(user);

                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction(nameof(Settings));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            var reloadData = await _accountService.GetProfileSettingsAsync(currentUserId);
            return View("Settings", reloadData);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                var profileData = await _accountService.GetProfileSettingsAsync(userId);
                return View("Settings", profileData);
            }

            var currentUserId = _userManager.GetUserId(User);
            var result = await _accountService.UpdatePasswordAsync(currentUserId, model);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Password updated successfully!";
                return RedirectToAction(nameof(Settings));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            var reloadData = await _accountService.GetProfileSettingsAsync(currentUserId);
            return View("Settings", reloadData);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAvatar(IFormFile avatarFile)
        {
            if (avatarFile == null || avatarFile.Length == 0)
            {
                TempData["ErrorMessage"] = "Please select a valid image file.";
                return RedirectToAction(nameof(Settings));
            }

            var currentUserId = _userManager.GetUserId(User);
            try
            {
                await  _accountService.UpdateAvatarAsync(currentUserId, avatarFile);

                var user = await _userManager.FindByIdAsync(currentUserId);
                await _signInManager.RefreshSignInAsync(user);

                TempData["SuccessMessage"] = "Avatar updated successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to upload avatar: " + ex.Message;
            }

            return RedirectToAction(nameof(Settings));
        }

        //  grade submission actions 


        [HttpGet]
        public async Task<IActionResult> GradeSubmission(int id)
        {
            var submission = await _submissionService.GetSubmissionByIdAsync(id);
            if (submission == null)
            {
                TempData["ErrorMessage"] = "Submission not found.";
                return RedirectToAction(nameof(Submissions));
            }
            // If already graded, redirect to submissions
            if (submission.Status == LMS.Domain.Enums.SubmissionStatus.Graded)
            {
                TempData["InfoMessage"] = "This submission has already been graded.";
                return RedirectToAction(nameof(Submissions));
            }

            var viewModel = new GradeSubmissionViewModel
            {
                Id = submission.Id,
                StudentName = submission.Student != null
                    ? $"{submission.Student.FirstName} {submission.Student.LastName}".Trim()
                    : "Unknown Student",
                StudentInitial = !string.IsNullOrEmpty(submission.Student?.FirstName)
                    ? submission.Student.FirstName.Substring(0, 1).ToUpper()
                    : "U",
                StudentAvatarColor = FormattingHelpers.GetAvatarColor(submission.Student?.FirstName),
                CourseTitle = submission.Assignment?.Title ?? "Untitled Course",
                AssignmentTitle = submission.Assignment?.Title ?? "Untitled Assignment",
                SubmittedAt = submission.SubmittedAt,
                SubmittedTimeAgo = FormattingHelpers.GetTimeAgo(submission.SubmittedAt),
                FileName = submission.SubmissionFiles?.FirstOrDefault()?.FileName,
                FileUrl = submission.SubmissionFiles?.FirstOrDefault()?.FileUrl,
                FileType = submission.SubmissionFiles?.FirstOrDefault()?.FileType,
                FileSize = submission.SubmissionFiles?.FirstOrDefault()?.FileSize,
                IsGraded = submission.Status == LMS.Domain.Enums.SubmissionStatus.Graded,
                StatusBadgeClass = submission.Status == LMS.Domain.Enums.SubmissionStatus.Graded
                    ? "bg-success-subtle text-success"
                    : "bg-warning-subtle text-warning",
                StatusText = submission.Status == LMS.Domain.Enums.SubmissionStatus.Graded
                    ? "Graded"
                    : "Pending Review",
                Grade = submission.Grade,
                Feedback = submission.Comment
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GradeSubmission(int id, int grade, string feedback)
        {
            if (grade < 0 || grade > 100)
            {
                TempData["ErrorMessage"] = "Grade must be between 0 and 100.";
                return RedirectToAction(nameof(GradeSubmission), new { id });
            }

            if (string.IsNullOrWhiteSpace(feedback))
            {
                TempData["ErrorMessage"] = "Feedback is required.";
                return RedirectToAction(nameof(GradeSubmission), new { id });
            }

            var result = await _submissionService.GradeSubmissionAsync(id, grade, feedback);
            if (!result)
            {
                TempData["ErrorMessage"] = "Failed to grade submission. Please try again.";
                return RedirectToAction(nameof(GradeSubmission), new { id });
            }

            TempData["SuccessMessage"] = $"Submission graded successfully with score {grade}/100!";
            return RedirectToAction(nameof(Submissions));
        }
    }
}