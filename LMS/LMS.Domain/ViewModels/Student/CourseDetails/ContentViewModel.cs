
namespace LMS.Domain.ViewModels.Student.CourseDetails
{
    public class ContentViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? VideoUrl { get; set; }
        public string? ArticleUrl { get; set; }
        public string? Text { get; set; }
        public bool IsCompleted { get; set; }
        public int CourseId { get; set; }
    }
}