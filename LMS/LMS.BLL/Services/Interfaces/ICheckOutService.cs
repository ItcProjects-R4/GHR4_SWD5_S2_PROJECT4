using LMS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using LMS.Domain.ViewModels.Student;

namespace LMS.BLL.Services.Interfaces
{
    public interface ICheckoutService
    {
        Task<CheckoutResponseViewModel> InitiateCheckoutAsync(int courseId, string studentId, string email, string name);
        Task<bool> ProcessPaymobWebhookAsync(string hmac, JsonElement payload);
        Task<IEnumerable<Payment>> GetStudentHistoryAsync(string studentId);
    }
}
