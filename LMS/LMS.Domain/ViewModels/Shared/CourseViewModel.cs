using LMS.Domain.ViewModels.Student.CourseDetails;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.ViewModels.Shared
{
    public class CourseViewModel
    {
        public int Id { get; set; }
        public  string Title { get; set; } = string.Empty;
        public  decimal Price { get; set; }
        public  string Description { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public List<ModuleViewModel> Modules { get; set; } = new();
        public int TotalLessonCount { get; set; }
    }
}
