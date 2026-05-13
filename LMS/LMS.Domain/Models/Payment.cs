using LMS.Domain.Enums;

namespace LMS.Domain.Models
{
    public class Payment
    {
        // Primary Key
        public int Id { get; set; }
        
        // Foreign Key
        public int? EnrollmentId { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus  Status { get; set; }
        
        public string TransactionId { get; set; }
        public DateTime PaidAt { get; set; }
        
        public Enrollment Enrollment { get; set; }
}
}
