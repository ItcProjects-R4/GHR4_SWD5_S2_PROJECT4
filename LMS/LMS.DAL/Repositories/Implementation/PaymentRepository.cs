using LMS.DAL.Data; // Update with your actual DbContext namespace
using LMS.DAL.Repositories.Interfaces;
using LMS.Domain.Enums;
using LMS.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace LMS.DAL.Repositories.Implementation
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Payment> AddPendingPaymentAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment> GetPaymentByIdAsync(int id)
        {
            return await _context.Payments.FindAsync(id);
        }

        public async Task UpdatePaymentStatusAsync(int paymentId, string transactionId, PaymentStatus status)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment != null)
            {
                payment.Status = status;
                payment.TransactionId = transactionId;
                await _context.SaveChangesAsync();
            }
        }

        public async Task CreateActiveEnrollmentAsync(string studentId, int courseId)
        {
            var enrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                Status = EnrollmentStatus.Active,
                EnrolledAt = DateTime.UtcNow
            };
            await _context.Enrollments.AddAsync(enrollment);
            await _context.SaveChangesAsync();
        }



        public async Task<IEnumerable<Payment>> GetStudentPurchaseHistoryAsync(string studentId)
        {
            return await _context.Payments
                .Include(p => p.Course) // We need this so the HTML can read the Course Title
                .Where(p => p.StudentId == studentId)
                .OrderByDescending(p => p.PaidAt)
                .ToListAsync();
        }

        // Inside LMS.DAL.Repositories.Implementation.PaymentRepository
        public async Task<IEnumerable<Payment>> GetAllPaymentsWithDetailsAsync()
        {
            return await _context.Payments
                .Include(p => p.Course)
                .Include(p => p.Student) // Include Student to get their Email for the table
                .OrderByDescending(p => p.PaidAt)
                .ToListAsync();
        }
    }
}