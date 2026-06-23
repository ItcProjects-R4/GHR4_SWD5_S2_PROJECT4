
namespace LMS.Domain.ViewModels.Student.Dashboard
{
    public class StudentDashboardViewModel
    {
        public string FirstName { get; set; } = string.Empty;
        public int EnrolledCoursesCount { get; set; }
        public int ActiveCoursesCount { get; set; }
        public int CompletedCoursesCount { get; set; }
        public List<ContinueLearningCourseViewModels> ContinueLearningCourses { get; set; } = [];
        public List<BrowseCourseViewModel> RecentlyAddedCourses { get; set; } = [];
    }
}