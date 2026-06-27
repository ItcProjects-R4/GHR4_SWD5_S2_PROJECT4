using LMS.Domain.Models;
using LMS.Domain.ViewModels.Instructor.CourseDetails;

namespace LMS.Domain.ViewModels.Instructor.CourseDetails
{
    public class CreateCoursePageViewModel
    {
        public CreateCourseViewModel CourseDetails { get; set; }
        public Course? Course { get; set; }
        public int Step { get; set; }

        // Strongly typed layout/alert properties
        public string? PageTitle { get; set; }
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }
    }
}