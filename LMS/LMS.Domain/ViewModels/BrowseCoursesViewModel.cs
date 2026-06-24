using LMS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.ViewModels
{
    public class BrowseCoursesViewModel
    {
        public IEnumerable<Course> Courses { get; set; } = new List<Course>();
        public string? SearchString { get; set; }
        public string? TypeFilter { get; set; }
        public string? SortOrder { get; set; }
    }
}
