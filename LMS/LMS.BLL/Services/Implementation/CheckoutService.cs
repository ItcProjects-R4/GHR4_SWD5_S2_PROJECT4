using LMS.BLL.Services.Interfaces;
using LMS.DAL.Repositories.Interfaces;
using LMS.Domain.Enums;
using LMS.Domain.Models;
using Microsoft.Extensions.Configuration; // If you use IConfiguration for the Iframe ID
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using LMS.Domain.ViewModels.Student;

namespace LMS.BLL.Services.Implementation
{
    public class CheckoutService : ICheckoutService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IPaymobService _paymobService;
        private readonly IConfiguration _config;
        private readonly INotificationService _notificationService;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;

        public CheckoutService(IPaymentRepository paymentRepository, 
            ICourseRepository courseRepository, 
            IPaymobService paymobService, 
            IConfiguration config,
            INotificationService notificationService,
            Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager)
        {
            _paymentRepository = paymentRepository;
            _courseRepository = courseRepository;
            _paymobService = paymobService;
            _config = config;
            _notificationService = notificationService;
            _userManager = userManager;
        }

        public async Task<CheckoutResponseViewModel> InitiateCheckoutAsync(int courseId, string studentId, string email, string name)
        {
            
                var course = await _courseRepository.GetCourseByIdAsync(courseId);
                if (course == null) return new CheckoutResponseViewModel { Success = false, ErrorMessage = "Course not found." };

                var response = new CheckoutResponseViewModel { Success = true, CourseTitle = course.Title, CourseId = course.Id };

                if (course.Price == 0)
                {
                    // Free Course
                    await _paymentRepository.CreateActiveEnrollmentAsync(studentId, courseId);
                    
                    await _notificationService.CreateAndSendToUserAsync(studentId, "Course Enrolled", $"You successfully bought the course {course.Title}", NotificationType.CoursePurchase);
                    await _notificationService.CreateAndSendToRoleAsync("Admin", "New Enrollment", $"Student {name} purchased the course {course.Title}", NotificationType.CoursePurchase);

                    response.IsFree = true;
                    return response;
                }

                // Paid Course
                var pendingPayment = new Payment
                {
                    CourseId = courseId,
                    StudentId = studentId,
                    Amount = course.Price,
                    Status = PaymentStatus.Pending,
                    PaidAt = DateTime.UtcNow
                };

                var savedPayment = await _paymentRepository.AddPendingPaymentAsync(pendingPayment);
                var uniqueMerchantOrderId = $"LMS-{savedPayment.Id}-{DateTime.UtcNow.Ticks}";
                var token = await _paymobService.GetPaymentKeyAsync(course.Price, email, name, "Student", uniqueMerchantOrderId);

                var iframeId = Environment.GetEnvironmentVariable("PAYMOB_IFRAME_ID") ?? _config["PAYMOB_IFRAME_ID"];
                response.IsFree = false;
                response.PaymobRedirectUrl = $"https://accept.paymob.com/api/acceptance/iframes/{iframeId}?payment_token={token}";

                return response;
            }

        public async Task<bool> ProcessPaymobWebhookAsync(string hmac, JsonElement payload)
        {
            if (!_paymobService.VerifyHmac(payload, hmac)) return false;
            var obj = payload.GetProperty("obj");
            bool success = obj.GetProperty("success").GetBoolean();
            string merchantOrderId = obj.GetProperty("order").GetProperty("merchant_order_id").GetString();
            string transactionId = obj.GetProperty("id").GetInt32().ToString();
            // 💡 Parse the database payment ID from our unique format "LMS-{paymentId}-{ticks}"
            int paymentId = 0;
            if (!string.IsNullOrEmpty(merchantOrderId) && merchantOrderId.StartsWith("LMS-"))
            {
                var parts = merchantOrderId.Split('-');
                if (parts.Length > 1)
                {
                    int.TryParse(parts[1], out paymentId);
                }
            }
            if (success && paymentId > 0)
            {
                var payment = await _paymentRepository.GetPaymentByIdAsync(paymentId);
                if (payment != null && payment.Status == PaymentStatus.Pending)
                {
                    await _paymentRepository.UpdatePaymentStatusAsync(paymentId, transactionId, PaymentStatus.Completed);
                    await _paymentRepository.CreateActiveEnrollmentAsync(payment.StudentId, payment.CourseId);

                    var student = await _userManager.FindByIdAsync(payment.StudentId);
                    var course = await _courseRepository.GetCourseByIdAsync(payment.CourseId);
                    
                    string studentName = student != null ? $"{student.FirstName} {student.LastName}" : "Unknown Student";
                    string courseTitle = course != null ? course.Title : "Unknown Course";

                    await _notificationService.CreateAndSendToUserAsync(payment.StudentId, "Purchase Successful", $"You successfully bought the course {courseTitle}", NotificationType.CoursePurchase);
                    await _notificationService.CreateAndSendToRoleAsync("Admin", "New Purchase", $"Student {studentName} purchased the course {courseTitle}", NotificationType.CoursePurchase);

                    return true;
                }
            }
            return false;
        }

        public async Task<IEnumerable<Payment>> GetStudentHistoryAsync(string studentId)
        {
            return await _paymentRepository.GetStudentPurchaseHistoryAsync(studentId);
        }
    }
}