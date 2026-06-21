
namespace LMS.Domain.ViewModels.Instructor.Enrollments
{
    public class InstructorEnrollmentViewModel
    {
        public int EnrollmentId { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public string StudentFirstName { get; set; } = string.Empty;
        public string StudentLastName { get; set; } = string.Empty;
        public string StudentFullName => $"{StudentFirstName} {StudentLastName}";
        public string StudentInitials =>
            (StudentFirstName.Length > 0 ? StudentFirstName[0].ToString() : "") +
            (StudentLastName.Length > 0 ? StudentLastName[0].ToString() : "");
        public string? StudentAvatarUrl { get; set; }
        public DateTime EnrolledAt { get; set; }
        public string EnrolledAtFormatted => EnrolledAt.ToString("MMM dd, yyyy");
        public string Status { get; set; } = string.Empty;

        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public int CompletedLessonsCount { get; set; }
        public int TotalLessonsCount { get; set; }
        public int ProgressPercentage => TotalLessonsCount > 0
            ? (int)Math.Round((double)CompletedLessonsCount * 100 / TotalLessonsCount)
            : 0;
    }
}