using LMS.BLL.Services.Implementation;
using LMS.BLL.Services.Interfaces;
using LMS.Domain.Models;
using LMS.Domain.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LMS.PL.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController(IStudentService studentService,
        IAccountService accountService, 
        SignInManager<ApplicationUser> signInManager,
         UserManager<ApplicationUser> userManager,
         ICheckoutService checkoutService
        )
    : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var studentDashboard = await studentService.GetStudentDashboardAsync();

            return View(studentDashboard);
        }

        [HttpGet]
        public async Task<IActionResult> PurchaseHistory()
        {
            var userId = userManager.GetUserId(User);
            var payments = await checkoutService.GetStudentHistoryAsync(userId);
            return View(payments);
        }

        [HttpGet]
        public async Task<IActionResult> Courses(string status, string search)
        {
            var enrolledCourses = await studentService.GetEnrolledCoursesAsync(status, search);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_CourseList", enrolledCourses);

            return View(enrolledCourses);
        }

        [HttpGet]
        public async Task<IActionResult> BrowseCourses()
        {
            var browseCourses = await studentService.GetBrowseCoursesAsync();

            return View(browseCourses);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int courseId)
        {
            if (courseId <= 0)
            {
                TempData["ErrorMessage"] = "Invalid course selection.";
                return RedirectToAction(nameof(BrowseCourses));
            }

                var result = await studentService.EnrollCourseAsync(courseId);
                if (result.Success)
                {
                    if (result.IsFree)
                    {
                        TempData["SuccessMessage"] = "Successfully enrolled in the course!";
                        return RedirectToAction(nameof(WatchCourse), new { id = courseId });
                    }
                    else
                    {
                        return Redirect(result.PaymobRedirectUrl);
                    }
                }

                TempData["ErrorMessage"] = result.ErrorMessage ?? "Failed to enroll. You might already be enrolled or the course is unavailable.";
            return RedirectToAction(nameof(BrowseCourses));
        }

        [HttpGet]
        public async Task<IActionResult> WatchCourse(int id)
        {
            if (id <= 0)
                return RedirectToAction("NotFound", "Home");

            var page = await studentService.GetCourseDetailsPageAsync(id);

            return View(page);
        }

        [HttpGet]
        public async Task<IActionResult> ContentDetails(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid content selection.");

            var contentDetails = await studentService.GetContentAsync(id);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_Content", contentDetails);

            return View(contentDetails);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkCompleted(int courseId, int contentId)
        {
            if (courseId <= 0 || contentId <= 0)
                return BadRequest("Invalid course or content selection.");

            await studentService.MarkContentAsCompletedAsync(contentId, courseId);

            return RedirectToAction(nameof(WatchCourse), new { id = courseId });
        }

        [HttpGet]
        public async Task<IActionResult> AssignmentDetails(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid assignment selection.");

            var assignment = await studentService.GetAssignmentDetailsAsync(id);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_AssignmentDetails", assignment);

            return View(assignment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitAssignment(int id, List<IFormFile> submissionFiles)
        {
            if (id <= 0)
                return BadRequest("Invalid assignment selection.");

            if (submissionFiles == null || submissionFiles.Count == 0)
                return BadRequest("No files were selected for submission.");

            const long maxFileSize = 10 * 1024 * 1024;
            var allowedExtensions = new[] { ".pdf", ".docx", ".zip" };

            foreach (var file in submissionFiles)
            {
                if (file.Length > maxFileSize)
                    return BadRequest($"File '{file.FileName}' exceeds the maximum allowed size of 10MB.");

                var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
                if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
                    return BadRequest($"File '{file.FileName}' has an invalid extension. Only .pdf, .docx, and .zip files are allowed.");
            }

            try
            {
                var result = await studentService.SubmitAssignmentAsync(id, submissionFiles);
                if (result != null)
                    return Ok();

                return StatusCode(500, "An error occurred while submitting the assignment.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Settings()
        {
            var userId = userManager.GetUserId(User);
            var profileData = await accountService.GetProfileSettingsAsync(userId);

            return View(profileData);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UpdateProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // If validation fails, reload settings data to re-render the view
                var userId = userManager.GetUserId(User);
                var profileData = await accountService.GetProfileSettingsAsync(userId);
                return View("Settings", profileData);
            }

            var currentUserId = userManager.GetUserId(User);
            var result = await accountService.UpdateProfileAsync(currentUserId, model);

            if (result.Succeeded)
            {
                //refresh the cookie claims immediately so the navbar updates
                var user = await userManager.FindByIdAsync(currentUserId);
                await signInManager.RefreshSignInAsync(user);

                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction(nameof(Settings));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            var reloadData = await accountService.GetProfileSettingsAsync(currentUserId);
            return View("Settings", reloadData);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var userId = userManager.GetUserId(User);
                var profileData = await accountService.GetProfileSettingsAsync(userId);
                return View("Settings", profileData);
            }

            var currentUserId = userManager.GetUserId(User);
            var result = await accountService.UpdatePasswordAsync(currentUserId, model);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Password updated successfully!";
                return RedirectToAction(nameof(Settings));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            var reloadData = await accountService.GetProfileSettingsAsync(currentUserId);
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

            var currentUserId = userManager.GetUserId(User);
            try
            {
                //upload to Cloudinary and update db
                await accountService.UpdateAvatarAsync(currentUserId, avatarFile);

                //refresh claims so the navbar profile picture updates immediately
                var user = await userManager.FindByIdAsync(currentUserId);
                await signInManager.RefreshSignInAsync(user);

                TempData["SuccessMessage"] = "Avatar updated successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to upload avatar: " + ex.Message;
            }

            return RedirectToAction(nameof(Settings));
        }



    }
}