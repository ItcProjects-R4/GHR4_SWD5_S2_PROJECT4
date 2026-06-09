using System;

namespace LMS.PL.ViewModels
{
    public class SubmissionListItemViewModel
    {
        public int Id { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentInitial { get; set; } = "U";
        public string CourseTitle { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }
        public bool IsGraded { get; set; }
        public int Grade { get; set; }
        public string Feedback { get; set; } = string.Empty;
    }
}