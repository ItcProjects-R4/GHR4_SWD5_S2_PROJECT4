using LMS.Domain.Models;
using System.Collections.Generic;

namespace LMS.BLL.ViewModels.Instructor.CourseDetails
{
    public class SubmissionsPageViewModel
    {
        public IEnumerable<Submission> Submissions { get; set; }
        public string? SearchString { get; set; }
        public string? StatusFilter { get; set; }
    }
}