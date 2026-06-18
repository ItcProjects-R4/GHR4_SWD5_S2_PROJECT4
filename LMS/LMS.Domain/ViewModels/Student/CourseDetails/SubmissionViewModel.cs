
namespace LMS.BLL.ViewModels.Student.CourseDetails
{
    public class SubmissionViewModel
    {
        public int Id { get; set; }
        public DateTime SubmittedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? Grade { get; set; }
        public string SubmissionStatus { get; set; } = string.Empty;
        public string? Comment { get; set; }
        public List<SubmissionFileViewModel> SubmissionFiles { get; set; } = new();
    }
}