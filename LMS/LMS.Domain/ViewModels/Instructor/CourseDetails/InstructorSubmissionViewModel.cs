using LMS.BLL.ViewModels.Student.CourseDetails;

namespace LMS.BLL.ViewModels.Instructor.CourseDetails
{
    public class InstructorSubmissionViewModel
    {
        public int Id { get; set; }
        public DateTime SubmittedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public string StudentFullName { get; set; } = string.Empty;
        public string? StudentAvatarUrl { get; set; }
        public int? Grade { get; set; }
        public string SubmissionStatus { get; set; } = string.Empty;
        public string? Comment { get; set; }
        public List<SubmissionFileViewModel> SubmissionFiles { get; set; } = [];
    }
}