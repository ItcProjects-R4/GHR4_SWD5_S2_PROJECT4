
namespace LMS.Domain.ViewModels.Student.CourseDetails
{
    public class SubmissionFileViewModel
    {
        public int Id { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public double? FileSize { get; set; }
    }
}