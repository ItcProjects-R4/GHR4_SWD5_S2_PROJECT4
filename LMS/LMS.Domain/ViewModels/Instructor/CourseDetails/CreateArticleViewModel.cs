using System.ComponentModel.DataAnnotations;

namespace LMS.BLL.ViewModels.Instructor.CourseDetails
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
    }
}