using LMS.Domain.ViewModels.Student.CourseDetails;

namespace LMS.Domain.ViewModels.Instructor.CourseDetails
{
    public class InstructorCourseDetailsPageViewModel
    {
        public CourseDetailsViewModel Course { get; set; } = new();
        public int? ActiveContentId { get; set; }
        public ContentViewModel? ActiveContent { get; set; }
        public int TotalContents { get; set; }
        public int TotalModules { get; set; }
        public int EnrolledStudentsCount { get; set; }
        public int CompletedStudentsCount { get; set; }
        public double AverageProgressPercent { get; set; }
    }
}