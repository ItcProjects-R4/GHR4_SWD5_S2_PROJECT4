using System;
using System.Collections.Generic;
using System.Text;

namespace LMSPhase01.Models
{
    public class Payment
{
    public int Id { get; set; }        // Primary Key
        
    // Foreign Key
    public int EnrollmentId { get; set; }

    public decimal Amount { get; set; }
    public string Status { get; set; }
    public string PaymentMethod { get; set; }
    public string TransactionId { get; set; }
    public DateTime PaidAt { get; set; }

    public Enrollment Enrollment { get; set; }
}
}
