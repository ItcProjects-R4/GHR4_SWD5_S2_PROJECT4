
namespace LMS.Domain.ViewModels.Student.CourseDetails
{
    public class AssignmentViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? FileUrl { get; set; }
        public DateTime DueDate { get; set; }
        public int MaxScore { get; set; }
        public SubmissionViewModel? Submission { get; set; }
    }
}