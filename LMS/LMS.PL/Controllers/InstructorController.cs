using LMS.BLL.Services.Interfaces;
using LMS.BLL.Services.Implementation;

using LMS.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LMS.Domain.ViewModels.Instructor.CourseDetails;

using LMS.Domain.ViewModels.Assistant;
using LMS.Domain.ViewModels.Account;

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
        public async Task<IActionResult> Dashboard()
        {
            var dashboard = await _instructorService.GetInstructorDashboardAsync();
            return View(dashboard);
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
        public async Task<IActionResult> WatchCourse(int id)
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
        public async Task<IActionResult> Courses(string? searchString, string sortBy = "newest", string? successMessage = null, string? errorMessage = null)
        {
            var instructorId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(instructorId)) return Challenge();

            var courses = await _instructorService.GetInstructorCoursesAsync(instructorId, searchString, sortBy);
            return View(new CoursesPageViewModel
            {
                Courses = courses,
                SearchString = searchString,
                SortBy = sortBy,
                SuccessMessage = successMessage,
                ErrorMessage = errorMessage,
                PageTitle = "Course Management"
            });
        }

        [HttpGet]
        public async Task<IActionResult> CreateCourse(int? id, int step = 1, string? successMessage = null, string? errorMessage = null)
        {
            var instructorId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(instructorId)) return Challenge();

            if (step == 2 && id.HasValue)
            {
                var course = await _instructorService.GetCourseForEditAsync(id.Value, instructorId);
                if (course == null) return NotFound();

                return View(new CreateCoursePageViewModel
                {
                    Step = 2,
                    Course = course,
                    CourseDetails = new CreateCourseViewModel
                    {
                        Title = course.Title,
                        Description = course.Description,
                        Price = course.Price,
                        ExistingThumbnailUrl = course.ThumbnailUrl
                    },
                    SuccessMessage = successMessage,
                    ErrorMessage = errorMessage,
                    PageTitle = "Curriculum Builder"
                });
            }

            return View(new CreateCoursePageViewModel
            {
                Step = 1,
                CourseDetails = new CreateCourseViewModel(),
                SuccessMessage = successMessage,
                ErrorMessage = errorMessage,
                PageTitle = "Create New Course"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCourse(CreateCoursePageViewModel model, int step = 1, int? id = null)
        {
            var instructorId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(instructorId)) return Challenge();

            if (step == 2 && id.HasValue)
            {
                // Action handles basic details update from Step 2
                if (!ModelState.IsValid)
                {
                    var course = await _instructorService.GetCourseForEditAsync(id.Value, instructorId);
                    model.Course = course;
                    model.Step = 2;
                    model.ErrorMessage = "Please correct the errors in the form.";
                    model.PageTitle = "Curriculum Builder";
                    return View(model);
                }

                var updateResult = await _instructorService.UpdateCourseAsync(id.Value, model.CourseDetails, instructorId);
                string? sMsg = null;
                string? eMsg = null;
                if (updateResult)
                {
                    sMsg = "Course details updated successfully!";
                }
                else
                {
                    eMsg = "Failed to update course details.";
                }
                return RedirectToAction(nameof(CreateCourse), new { id = id.Value, step = 2, successMessage = sMsg, errorMessage = eMsg });
            }

            // Step 1 Basics form submission
            if (!ModelState.IsValid)
            {
                model.Step = 1;
                model.ErrorMessage = "Please correct the errors in the form.";
                model.PageTitle = "Create New Course";
                return View(model);
            }

            var newCourse = await _instructorService.CreateCourseAsync(model.CourseDetails, instructorId);
            return RedirectToAction(nameof(CreateCourse), new { id = newCourse.Id, step = 2, successMessage = "Course basics saved successfully!" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var instructorId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(instructorId)) return Challenge();

            var result = await _instructorService.DeleteCourseAsync(id, instructorId);
            if (result)
            {
                return RedirectToAction(nameof(Courses), new { successMessage = "Course deleted successfully!" });
            }
            else
            {
                return RedirectToAction(nameof(Courses), new { errorMessage = "Failed to delete course." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddModule(int courseId, string moduleTitle)
        {
            if (string.IsNullOrEmpty(moduleTitle))
            {
                return RedirectToAction(nameof(CreateCourse), new { id = courseId, step = 2, errorMessage = "Module title cannot be empty." });
            }

            await _instructorService.AddModuleAsync(courseId, moduleTitle);
            return RedirectToAction(nameof(CreateCourse), new { id = courseId, step = 2, successMessage = "Module added successfully." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteModule(int id, int courseId)
        {
            var instructorId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(instructorId)) return Challenge();

            var result = await _instructorService.DeleteModuleAsync(id, courseId, instructorId);
            if (result)
            {
                return RedirectToAction(nameof(CreateCourse), new { id = courseId, step = 2, successMessage = "Module deleted successfully!" });
            }
            else
            {
                return RedirectToAction(nameof(CreateCourse), new { id = courseId, step = 2, errorMessage = "Failed to delete module." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddContent(int moduleId, int courseId, CreateContentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(CreateCourse), new { id = courseId, step = 2, errorMessage = "Failed to add content. Please check inputs." });
            }

            await _instructorService.AddContentAsync(moduleId, model);
            return RedirectToAction(nameof(CreateCourse), new { id = courseId, step = 2, successMessage = "Lesson content added successfully!" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteContent(int id, int courseId)
        {
            var instructorId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(instructorId)) return Challenge();

            var result = await _instructorService.DeleteContentAsync(id, courseId, instructorId);
            if (result)
            {
                return RedirectToAction(nameof(CreateCourse), new { id = courseId, step = 2, successMessage = "Lesson deleted successfully!" });
            }
            else
            {
                return RedirectToAction(nameof(CreateCourse), new { id = courseId, step = 2, errorMessage = "Failed to delete lesson." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAssignment(int moduleId, int courseId, string title, DateTime dueDate, int maxScore, IFormFile? resourceFile)
        {
            if (string.IsNullOrEmpty(title))
            {
                return RedirectToAction(nameof(CreateCourse), new { id = courseId, step = 2, errorMessage = "Assignment title is required." });
            }

            await _instructorService.AddAssignmentAsync(moduleId, title, dueDate, maxScore, resourceFile);
            return RedirectToAction(nameof(CreateCourse), new { id = courseId, step = 2, successMessage = "Assignment added successfully!" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAssignment(int id, int courseId)
        {
            var instructorId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(instructorId)) return Challenge();

            var result = await _instructorService.DeleteAssignmentAsync(id, courseId, instructorId);
            if (result)
            {
                return RedirectToAction(nameof(CreateCourse), new { id = courseId, step = 2, successMessage = "Assignment deleted successfully!" });
            }
            else
            {
                return RedirectToAction(nameof(CreateCourse), new { id = courseId, step = 2, errorMessage = "Failed to delete assignment." });
            }
        }

        [HttpGet]
        public IActionResult CreateArticle(int moduleId, int courseId)
        {
            return View(new CreateArticleViewModel
            {
                ModuleId = moduleId,
                CourseId = courseId,
                PageTitle = "Create Article"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateArticle(CreateArticleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.PageTitle = "Create Article";
                return View(model);
            }

            var contentModel = new CreateContentViewModel
            {
                Title = model.Title,
                Text = model.Text,
                ContentType = "text"
            };

            await _instructorService.AddContentAsync(model.ModuleId, contentModel);
            return RedirectToAction(nameof(CreateCourse), new { id = model.CourseId, step = 2, successMessage = "Text Article lesson created successfully!" });
        }

        [HttpGet]
        public async Task<IActionResult> Submissions(string? searchString, string? statusFilter, string? successMessage = null, string? errorMessage = null)
        {
            var instructorId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(instructorId)) return Challenge();

            var submissions = await _instructorService.GetSubmissionsQueueAsync(instructorId, searchString, statusFilter);
            return View(new SubmissionsPageViewModel
            {
                Submissions = submissions,
                SearchString = searchString,
                StatusFilter = statusFilter,
                SuccessMessage = successMessage,
                ErrorMessage = errorMessage,
                PageTitle = "Student Submissions"
            });
        }

        [HttpGet]
        public async Task<IActionResult> Grade(int id)
        {
            var instructorId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(instructorId)) return Challenge();

            var submission = await _instructorService.GetSubmissionForGradingAsync(id, instructorId);
            if (submission == null) return NotFound();

            var file = submission.SubmissionFiles?.FirstOrDefault();
            var viewModel = new GradeSubmissionViewModel
            {
                SubmissionId = submission.Id,
                Grade = submission.Grade ?? 0,
                Comment = submission.Comment ?? string.Empty,
                StudentName = $"{submission.Student.FirstName} {submission.Student.LastName}",
                StudentAvatarUrl = submission.Student.AvatarUrl ?? string.Empty,
                AssignmentTitle = submission.Assignment.Title,
                CourseTitle = submission.Assignment.Module.Course.Title,
                SubmittedFileName = file?.FileName,
                SubmittedFileUrl = file?.FileUrl,
                PageTitle = "Grade Workspace"
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Grade(GradeSubmissionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.PageTitle = "Grade Workspace";
                return View(model);
            }

            var instructorId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(instructorId)) return Challenge();

            var result = await _instructorService.GradeSubmissionAsync(model.SubmissionId, model.Grade, model.Comment, instructorId);
            string? sMsg = null;
            string? eMsg = null;
            if (result)
            {
                sMsg = $"{model.StudentName} graded successfully!";
            }
            else
            {
                eMsg = "Failed to submit grade.";
            }

            return RedirectToAction(nameof(Submissions), new { successMessage = sMsg, errorMessage = eMsg });
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