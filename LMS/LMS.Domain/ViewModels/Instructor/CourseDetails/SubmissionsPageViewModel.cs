using LMS.Domain.Models;
using System.Collections.Generic;

namespace LMS.Domain.ViewModels.Instructor.CourseDetails
{
    public class SubmissionsPageViewModel
    {
        public IEnumerable<Submission> Submissions { get; set; }
        public string? SearchString { get; set; }
        public string? StatusFilter { get; set; }

        // Strongly typed layout/alert properties
        public string? PageTitle { get; set; }
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }
    }
}