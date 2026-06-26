using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.ViewModels.Home
{
    public class AboutViewModel
    {
        public string InstructorName { get; set; }
        public string? Biography { get; set; }
        public string? AvatarUrl { get; set; }

        public string? Title { get; set; }
        public int CoursesCount { get; set; }
        public int StudentsCount { get; set; }
    }
}
