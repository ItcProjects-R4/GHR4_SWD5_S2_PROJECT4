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
            //bypassing payment locally temp disabled for live presentation
            //if (Request.Host.Host == "localhost" || Request.Host.Host == "127.0.0.1")
            //{
            //    // Prevent duplicate enrollments locally
            //    var isEnrolledLocal = await _context.Enrollments.AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId);
            //    if (isEnrolledLocal)
            //    {
            //        return RedirectToAction("WatchCourse", "Student", new { id = courseId });
            //    }
            //    // Instantly create the pending payment
            //    await _checkoutService.InitiateCheckoutAsync(courseId, studentId, "student@test.com", "Local Student");

            //    // Find the payment and set it to completed
            //    var payment = await _context.Payments
            //        .FirstOrDefaultAsync(p => p.StudentId == studentId && p.CourseId == courseId && p.Status == Domain.Enums.PaymentStatus.Pending);
            //    if (payment != null)
            //    {
            //        payment.Status = Domain.Enums.PaymentStatus.Completed;
            //        payment.TransactionId = "LOCAL_TEST_" + Guid.NewGuid().ToString().Substring(0, 8);
            //        // Create the active enrollment locally
            //        var enrollment = new Domain.Models.Enrollment
            //        {
            //            StudentId = studentId,
            //            CourseId = courseId,
            //            Status = Domain.Enums.EnrollmentStatus.Active,
            //            EnrolledAt = DateTime.UtcNow
            //        };
            //        await _context.Enrollments.AddAsync(enrollment);
            //        await _context.SaveChangesAsync();
            //    }
            //    TempData["SuccessMessage"] = "Local development bypass: Enrolled successfully!";
            //    return RedirectToAction("WatchCourse", "Student", new { id = courseId });
            //}
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

            if (!result.Success)
            {
                TempData["ErrorMessage"] = $"Payment initiation failed: {result.ErrorMessage}";
                return RedirectToAction("Checkout", new { courseId });
            }

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
        public async Task<IActionResult> PaymentSuccess([FromQuery] string? success)
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var lastEnrollment = await _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.StudentId == studentId)
                .OrderByDescending(e => e.EnrolledAt)
                .FirstOrDefaultAsync();
            int courseId = 0;
            string courseTitle = "your new course";
            if (lastEnrollment != null)
            {
                courseId = lastEnrollment.CourseId;
                courseTitle = lastEnrollment.Course.Title;
            }
            ViewBag.CourseId = courseId;
            ViewBag.CourseTitle = courseTitle;
            return View();
        }
    }
}