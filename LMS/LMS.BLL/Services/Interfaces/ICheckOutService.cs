using LMS.Domain.ViewModels;
using LMS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace LMS.BLL.Services.Interfaces
{
    public interface ICheckoutService
    {
        Task<CheckoutResponse> InitiateCheckoutAsync(int courseId, string studentId, string email, string name);
        Task<bool> ProcessPaymobWebhookAsync(string hmac, JsonElement payload);
        Task<IEnumerable<Payment>> GetStudentHistoryAsync(string studentId);
    }
}
