using LMSPhase01.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMSPhase01.Models
{
    public class Enrollment
    { 
        
    public int Id { get; set; }    // Primary Key

    // Foreign Keys
    public string UserId { get; set; }
    public int CourseId { get; set; }

    public DateTime EnrolledAt { get; set; }
    public EnrollmentStatus Status { get; set; } 

    // Navigation Properties
    public ApplicationUser User { get; set; }
    public Course Course { get; set; }
    public Payment Payment { get; set; }

    }
}
