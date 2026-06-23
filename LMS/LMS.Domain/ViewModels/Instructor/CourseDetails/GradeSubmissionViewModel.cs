using System.ComponentModel.DataAnnotations;

namespace LMS.BLL.ViewModels.Instructor.CourseDetails
{
    public class GradeSubmissionViewModel
    {
        public int SubmissionId { get; set; }

        [Required(ErrorMessage = "Score is required.")]
        [Range(0, 100, ErrorMessage = "Score must be between 0 and 100.")]
        public int Grade { get; set; }

        [Required(ErrorMessage = "Feedback is required.")]
        [StringLength(1000, ErrorMessage = "Feedback cannot exceed 1000 characters.")]
        public string Comment { get; set; }

        // Display properties
        public string StudentName { get; set; }
        public string StudentAvatarUrl { get; set; }
        public string AssignmentTitle { get; set; }
        public string CourseTitle { get; set; }
        public string? SubmittedFileName { get; set; }
        public string? SubmittedFileUrl { get; set; }
    }
}