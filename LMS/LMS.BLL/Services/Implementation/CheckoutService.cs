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

        public CheckoutService(IPaymentRepository paymentRepository, ICourseRepository courseRepository, IPaymobService paymobService, IConfiguration config)
        {
            _paymentRepository = paymentRepository;
            _courseRepository = courseRepository;
            _paymobService = paymobService;
            _config = config;
        }

        public async Task<CheckoutResponseViewModel> InitiateCheckoutAsync(int courseId, string studentId, string email, string name)
        {
            // Note: You need a way to get the course. Assuming your PaymentRepository or CourseRepository can fetch it.
            // For this example, let's pretend PaymentRepository has a GetCourseByIdAsync method.
                var course = await _courseRepository.GetCourseByIdAsync(courseId);
                if (course == null) return new CheckoutResponseViewModel { Success = false, ErrorMessage = "Course not found." };

                var response = new CheckoutResponseViewModel { Success = true, CourseTitle = course.Title };

                if (course.Price == 0)
                {
                    // BUSINESS LOGIC: Free Course
                    await _paymentRepository.CreateActiveEnrollmentAsync(studentId, courseId);
                    response.IsFree = true;
                    return response;
                }

                // BUSINESS LOGIC: Paid Course
                var pendingPayment = new Payment
                {
                    CourseId = courseId,
                    StudentId = studentId,
                    Amount = course.Price,
                    Status = PaymentStatus.Pending,
                    PaidAt = DateTime.UtcNow
                };

                var savedPayment = await _paymentRepository.AddPendingPaymentAsync(pendingPayment);
                var token = await _paymobService.GetPaymentKeyAsync(course.Price, email, name, "Student", savedPayment.Id.ToString());

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

            if (success && int.TryParse(merchantOrderId, out int paymentId))
            {
                var payment = await _paymentRepository.GetPaymentByIdAsync(paymentId);
                if (payment != null && payment.Status == PaymentStatus.Pending)
                {
                    await _paymentRepository.UpdatePaymentStatusAsync(paymentId, transactionId, PaymentStatus.Completed);
                    await _paymentRepository.CreateActiveEnrollmentAsync(payment.StudentId, payment.CourseId);
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