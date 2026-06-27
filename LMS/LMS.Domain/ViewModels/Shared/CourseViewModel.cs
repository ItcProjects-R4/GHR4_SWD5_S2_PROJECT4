using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.ViewModels.Shared
{
    public class CourseViewModel
    {
        public int Id { get; set; }
        public  string Title { get; set; }
        public  decimal Price { get; set; }
        public  string Description { get; set; }
    }
}
