using LMS.BLL.Services.Interfaces;
using LMS.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace LMS.PL.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ICheckoutService _checkoutService;
        private readonly ICourseRepository _courseRepository;

        public CheckoutController(ICheckoutService checkoutService, ICourseRepository courseRepository)
        {
            _checkoutService = checkoutService;
            _courseRepository = courseRepository;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Checkout(int courseId)
        {
            // Assuming you have access to a course repository to get the course details
            var course = await _courseRepository.GetCourseByIdAsync(courseId);

            if (course == null) return NotFound();

            return View(course);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Pay(int courseId)
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email) ?? "student@test.com";
            var name = User.Identity.Name ?? "Student";

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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> PurchaseHistory()
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var history = await _checkoutService.GetStudentHistoryAsync(studentId);
            return View(history);
        }

        [HttpGet]
        public IActionResult PaymentSuccess()
        {
            return View(); // Assumes TempData or a BLL call provides the course title in the view as established earlier
        }
    }
}