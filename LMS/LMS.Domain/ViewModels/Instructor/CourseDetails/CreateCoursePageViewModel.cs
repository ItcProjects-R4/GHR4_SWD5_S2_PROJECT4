using System;
using System.Collections.Generic;
using System.Text;
using LMS.Domain.Models;

namespace LMS.BLL.ViewModels.Instructor.CourseDetails
{
    public class CreateCoursePageViewModel
    {
        public CreateCourseViewModel CourseDetails { get; set; }
        public Course? Course { get; set; }
        public int Step { get; set; }
    }
}