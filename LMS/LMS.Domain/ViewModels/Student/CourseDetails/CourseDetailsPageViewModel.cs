
namespace LMS.Domain.ViewModels.Student.CourseDetails
{
    public class CourseDetailsPageViewModel
    {
        public CourseDetailsViewModel Course { get; set; } = new();
        public int? ActiveContentId { get; set; }
        public ContentViewModel? ActiveContent { get; set; }
        public int TotalContents { get; set; }
        public int CompletedContents { get; set; }
        public int TotalModules { get; set; }
        public int ProgressPercent { get; set; }
        public bool IsEnrolled { get; set; }
    }
}