using LMS.Domain.Models;
using LMS.Domain.Enums;
using System.Threading.Tasks;

namespace LMS.DAL.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment> AddPendingPaymentAsync(Payment payment);
        Task<Payment> GetPaymentByIdAsync(int id);
        Task UpdatePaymentStatusAsync(int paymentId, string transactionId, PaymentStatus status);
        Task CreateActiveEnrollmentAsync(string studentId, int courseId);
        Task<IEnumerable<Payment>> GetStudentPurchaseHistoryAsync(string studentId);

        //payments for instructor dashboard
        Task<IEnumerable<Payment>> GetAllPaymentsWithDetailsAsync();
    }
}