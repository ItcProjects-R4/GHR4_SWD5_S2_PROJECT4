
namespace LMS.Domain.ViewModels.Instructor.Enrollments
{
    public class CourseEnrollmentGroupViewModel
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public List<InstructorEnrollmentViewModel> Enrollments { get; set; } = new();
    }
}