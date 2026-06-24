
namespace LMS.Domain.ViewModels.Instructor.CourseDetails
{
    public class InstructorAssignmentDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? FileUrl { get; set; }
        public DateTime DueDate { get; set; }
        public int MaxScore { get; set; }
        public List<InstructorSubmissionViewModel> Submissions { get; set; } = [];
    }
}