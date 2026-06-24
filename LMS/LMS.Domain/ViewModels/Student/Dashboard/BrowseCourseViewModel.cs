
namespace LMS.Domain.ViewModels.Student.Dashboard
{
    public class BrowseCourseViewModel
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}