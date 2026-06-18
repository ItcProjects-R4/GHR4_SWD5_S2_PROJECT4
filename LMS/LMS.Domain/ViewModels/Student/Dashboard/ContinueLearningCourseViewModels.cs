
namespace LMS.BLL.ViewModels.Student.Dashboard
{
    public class ContinueLearningCourseViewModels
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public int ProgressPercentage { get; set; }
        public int CompletedLessonsCount { get; set; }
        public int TotalLessonsCount { get; set; }
    }
}