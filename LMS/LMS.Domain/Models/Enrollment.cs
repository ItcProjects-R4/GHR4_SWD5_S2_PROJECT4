using LMS.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

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
        // Navigation Properties
        [ForeignKey("StudentId")]
        public ApplicationUser Student { get; set; }

        [ForeignKey("CourseId")]
        public Course Course { get; set; }

        // 1-to-1 relationship back to the Payment that funded this enrollment
        public Payment? Payment { get; set; }

    }
}
