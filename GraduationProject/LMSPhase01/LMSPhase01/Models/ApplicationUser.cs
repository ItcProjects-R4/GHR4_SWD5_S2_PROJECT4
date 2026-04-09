using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LMSPhase01.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }
        public string? AvatarUrl { get; set; }
        public string Role { get; set; }
        public List<Course> Courses { get; set; }
        public List<Notification> Notifications { get; set; }
        public List<Submission> Submissions { get; set; }
        public List<Enrollment> Enrollments { get; set; }
        public List<Progress> Progresses { get; set; }

    }
}
