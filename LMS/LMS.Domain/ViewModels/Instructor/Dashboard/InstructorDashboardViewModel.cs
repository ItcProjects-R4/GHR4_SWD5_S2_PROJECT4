using System;
using System.Collections.Generic;

namespace LMS.Domain.ViewModels.Instructor.Dashboard
{
    public class InstructorDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public decimal TotalRevenue { get; set; }
        public int ActiveCourses { get; set; }
        public int TotalEnrollments { get; set; }

        public List<RecentEnrollmentViewModel> RecentEnrollments { get; set; } = new List<RecentEnrollmentViewModel>();
        public List<RecentSubmissionViewModel> RecentSubmissions { get; set; } = new List<RecentSubmissionViewModel>();
    }

    public class RecentEnrollmentViewModel
    {
        public string StudentInitials { get; set; }
        public string StudentName { get; set; }
        public string CourseTitle { get; set; }
        public decimal Amount { get; set; }
        public DateTime EnrolledAt { get; set; }
        public string AvatarBgColor { get; set; } // to randomly color the avatar if needed
    }

    public class RecentSubmissionViewModel
    {
        public string StudentInitials { get; set; }
        public string StudentName { get; set; }
        public string CourseAndModule { get; set; }
        public string Status { get; set; }
        public string AvatarBgColor { get; set; }
    }
}
