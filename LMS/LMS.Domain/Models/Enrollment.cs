using LMS.Domain.Enums;

namespace LMS.Domain.Models
{
    public class Enrollment
    {
        // Primary Key
        public int Id { get; set; }

        // Foreign Keys
        public string StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrolledAt { get; set; }
        public EnrollmentStatus Status { get; set; } 

        // Navigation Properties
        public ApplicationUser Student { get; set; }
        public Course Course { get; set; }
        public Payment Payment { get; set; }

    }
}
