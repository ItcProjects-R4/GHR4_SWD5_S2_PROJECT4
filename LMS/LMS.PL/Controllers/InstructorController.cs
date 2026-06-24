using LMS.BLL.Services.Implementation;
using LMS.BLL.Services.Interfaces;
using LMS.Domain.Models;
using LMS.Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LMS.PL.Controllers
{
    [Authorize(Roles = "Instructor")]
    public class InstructorController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IReportingService _reportingService;
        private readonly IInstructorService _instructorService;
        private readonly IAccountService _accountService;
        private readonly SignInManager<ApplicationUser> _signInManager;


        public InstructorController(IStudentService studentService,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager, 
        IReportingService reportingService,
        IInstructorService instructorService,
        IAccountService accountService,
        SignInManager<ApplicationUser> signInManager)
        {
            _studentService = studentService;
            _userManager = userManager;
            _roleManager = roleManager;
            _reportingService = reportingService;
            _instructorService = instructorService;
            _accountService = accountService;
            _signInManager = signInManager;
        }

        [HttpGet]
        public async Task<IActionResult> Users(string? searchString, string? roleFilter)
        {
            var users = await _studentService.GetFilteredUsersAsync(searchString ?? string.Empty, roleFilter ?? string.Empty);
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAssistant(CreateAssistantViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var users = await _studentService.GetFilteredUsersAsync(string.Empty, string.Empty);
                return View("Users", users);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(user, model.Password);
            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                var users = await _studentService.GetFilteredUsersAsync(string.Empty, string.Empty);
                return View("Users", users);
            }

            if (!await _roleManager.RoleExistsAsync("Assistant"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Assistant"));
            }

            await _userManager.AddToRoleAsync(user, "Assistant");

            if (model.SelectedPermissions != null)
            {
                foreach (var permission in model.SelectedPermissions)
                {
                    await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("Permission", permission));
                }
            }

            TempData["SuccessMessage"] = "Assistant account created successfully!";
            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePermissions(string userId, System.Collections.Generic.List<string>? selectedPermissions)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var existingClaims = await _userManager.GetClaimsAsync(user);
            var permissionClaims = existingClaims.Where(c => c.Type == "Permission");

            foreach (var claim in permissionClaims)
            {
                await _userManager.RemoveClaimAsync(user, claim);
            }

            if (selectedPermissions != null)
            {
                foreach (var permission in selectedPermissions)
                {
                    await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("Permission", permission));
                }
            }

            TempData["SuccessMessage"] = "Permissions updated successfully!";
            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "User deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete user.";
            }

            return RedirectToAction(nameof(Users));
        }

        [HttpGet]
        public async Task<IActionResult> Payments()
        {
            var reports = await _reportingService.GetFinancialReportsAsync();
            return View("Payments", reports);
        }
    

        [HttpGet]
        public async Task<IActionResult> Enrollments(string search)
        {
            var enrollments = await _instructorService.GetEnrollmentsAsync(search);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_EnrollmentList", enrollments);

            return View(enrollments);
        }

        [HttpGet]
        public async Task<IActionResult> CourseDetails(int id)
        {
            if (id <= 0)
                return RedirectToAction("NotFound", "Home");

            var page = await _instructorService.GetCourseDetailsPageAsync(id);
            return View(page);
        }

        [HttpGet]
        public async Task<IActionResult> ContentDetails(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid content selection.");

            var contentDetails = await _instructorService.GetContentAsync(id);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_Content", contentDetails);

            return View(contentDetails);
        }

        [HttpGet]
        public async Task<IActionResult> AssignmentDetails(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid assignment selection.");

            var assignment = await _instructorService.GetAssignmentDetailsAsync(id);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_AssignmentDetails", assignment);

            return View(assignment);

        }

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
                // If validation fails, reload settings data to re-render the view
                var userId = _userManager.GetUserId(User);
                var profileData = await _accountService.GetProfileSettingsAsync(userId);
                return View("Settings", profileData);
            }

            var currentUserId = _userManager.GetUserId(User);
            var result = await _accountService.UpdateProfileAsync(currentUserId, model);

            if (result.Succeeded)
            {
                //refresh the cookie claims immediately so the navbar updates
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
                //upload to Cloudinary and update db
                await _accountService.UpdateAvatarAsync(currentUserId, avatarFile);

                //refresh claims so the navbar profile picture updates immediately
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



    }
}