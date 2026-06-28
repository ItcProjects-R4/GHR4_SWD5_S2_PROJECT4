using LMS.BLL.Services.Interfaces;
using LMS.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace LMS.PL.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ICheckoutService _checkoutService;
        private readonly ICourseRepository _courseRepository;
        private readonly DAL.Data.IApplicationDbContext _context;


        public CheckoutController(ICheckoutService checkoutService, ICourseRepository courseRepository, DAL.Data.IApplicationDbContext context)
        {
            _checkoutService = checkoutService;
            _courseRepository = courseRepository;
            _context = context;

        }

        [Authorize(Roles = "Student")]
        [HttpGet]
        public async Task<IActionResult> Checkout(int courseId)
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Redirect if already enrolled
            var isEnrolled = await _context.Enrollments.AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId);
            if (isEnrolled)
            {
                return RedirectToAction("WatchCourse", "Student", new { id = courseId });
            }
            var course = await _courseRepository.GetCourseByIdAsync(courseId);
            if (course == null) return NotFound();

            var viewModel = new Domain.ViewModels.Shared.CourseViewModel
            {
                Id = course.Id,
                Title = course.Title,
                Price = course.Price,
                Description = course.Description
            };
            return View(viewModel);
        }

        [Authorize(Roles = "Student")]
        [HttpPost]
        public async Task<IActionResult> Pay(int courseId)
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Redirect if already enrolled
            var isEnrolled = await _context.Enrollments.AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId);
            if (isEnrolled)
            {
                return RedirectToAction("WatchCourse", "Student", new { id = courseId });
            }
            var emailClaim = User.FindFirstValue(ClaimTypes.Email);
            var email = string.IsNullOrWhiteSpace(emailClaim) ? "student@test.com" : emailClaim;

            var nameClaim = User.Identity?.Name;
            var name = string.IsNullOrWhiteSpace(nameClaim) ? "Student" : nameClaim;
            var result = await _checkoutService.InitiateCheckoutAsync(courseId, studentId, email, name);
            if (!result.Success) return NotFound(result.ErrorMessage);
            if (result.IsFree)
            {
                TempData["CourseTitle"] = result.CourseTitle;
                return RedirectToAction("PaymentSuccess");
            }
            return Redirect(result.PaymobRedirectUrl);
        }

        [HttpPost("api/paymob/callback")]
        public async Task<IActionResult> Callback([FromQuery] string hmac, [FromBody] JsonElement payload)
        {
            bool isSuccess = await _checkoutService.ProcessPaymobWebhookAsync(hmac, payload);
            if (!isSuccess) return BadRequest("Invalid Webhook");

            return Ok();
        }

        [Authorize(Roles = "Student")]
        [HttpGet]
        public async Task<IActionResult> PurchaseHistory()
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var history = await _checkoutService.GetStudentHistoryAsync(studentId);
            return View(history);
        }

        [Authorize(Roles = "Student")]
        [HttpGet]
        public IActionResult PaymentSuccess()
        {
            return View(); // Assumes TempData or a BLL call provides the course title in the view as established earlier
        }
    }
}