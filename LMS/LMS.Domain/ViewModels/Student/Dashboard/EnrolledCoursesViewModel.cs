
namespace LMS.Domain.ViewModels.Student.Dashboard
{
    public class EnrolledCoursesViewModel
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string EnrollmentStatus { get; set; } = string.Empty;
        public int ProgressPercentage { get; set; }
        public int CompletedLessonsCount { get; set; }
        public int TotalLessonsCount { get; set; }
    }
}