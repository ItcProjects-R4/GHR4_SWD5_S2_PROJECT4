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
    public enum Status  {Pending, Completed, Failed, Refunded}
    public enum PaymentMethod { Card, VodafoneCash, PayPal }
    public string TransactionId { get; set; }
    public DateTime PaidAt { get; set; }

    public Enrollment Enrollment { get; set; }
}
}
