using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;


namespace LMS.Domain.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(50)]
        public required string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public required string LastName { get; set; }
        public string? AvatarUrl { get; set; }

        public string? Biography { get; set; }


        //waiting for other models to be created to add these navigation properties
        public List<Course> Courses { get; set; }
        //public List<Notification> Notifications { get; set; }
        //public List<Submission> Submissions { get; set; }
        public List<Enrollment> Enrollments { get; set; }
        public List<Payment> Payments { get; set; }
        public List<Progress> Progresses { get; set; }

        public List<Submission> Submissions { get; set; }

    }
}
