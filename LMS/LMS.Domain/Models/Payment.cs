using LMS.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Domain.Models
{
    public class Payment
    {
        // Primary Key
        public int Id { get; set; }
        
        [Required]
        public string StudentId { get; set; }

        // The payment MUST know what course they are buying
        [Required]
        public int CourseId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public PaymentStatus Status { get; set; }

        // Nullable because it won't exist until Paymob confirms it
        public string? TransactionId { get; set; }

        public DateTime PaidAt { get; set; }

        // Navigation Properties
        [ForeignKey("StudentId")]
        public ApplicationUser Student { get; set; }

        [ForeignKey("CourseId")]
        public Course Course { get; set; }

        // Optional: 1-to-1 link to the final enrollment (can be nullable)
        public int? EnrollmentId { get; set; }
        [ForeignKey("EnrollmentId")]
        public Enrollment? Enrollment { get; set; }
    }
}
