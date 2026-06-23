using LMS.Domain.Models;
using System.Collections.Generic;

namespace LMS.BLL.ViewModels.Instructor.CourseDetails
{
    public class CoursesPageViewModel
    {
        public IEnumerable<Course> Courses { get; set; }
        public string? SearchString { get; set; }
        public string SortBy { get; set; }
    }
}