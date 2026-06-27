using System.ComponentModel.DataAnnotations;

namespace LMS.Domain.ViewModels.Instructor.CourseDetails
{
    public class CreateArticleViewModel
    {
        public int ModuleId { get; set; }
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        public string Text { get; set; }

        // Strongly typed layout/alert properties
        public string? PageTitle { get; set; }
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }
    }
}