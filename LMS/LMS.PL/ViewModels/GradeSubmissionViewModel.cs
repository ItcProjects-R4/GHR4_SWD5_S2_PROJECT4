using System;
using System.ComponentModel.DataAnnotations;

namespace LMS.PL.ViewModels
{
    public class GradeSubmissionViewModel
    {
        public int Id { get; set; }

        public string StudentName { get; set; } = string.Empty;
        public string StudentInitial { get; set; } = "U";
        public string StudentAvatarColor { get; set; } = "var(--accent-color)";

        public string CourseTitle { get; set; } = string.Empty;
        public string AssignmentTitle { get; set; } = string.Empty;

        public DateTime SubmittedAt { get; set; }
        public string SubmittedTimeAgo { get; set; } = string.Empty;

        public string? FileName { get; set; }
        public string? FileUrl { get; set; }
        public string? FileType { get; set; }
        public double? FileSize { get; set; }

        public bool IsGraded { get; set; }
        public string StatusBadgeClass { get; set; } = "bg-warning-subtle text-warning";
        public string StatusText { get; set; } = "Pending Review";

        [Range(0, 100, ErrorMessage = "Grade must be between 0 and 100")]
        public int? Grade { get; set; }

        public string? Feedback { get; set; }
    }
}