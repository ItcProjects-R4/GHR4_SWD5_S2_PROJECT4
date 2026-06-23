using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace LMS.BLL.ViewModels.Instructor.CourseDetails
{
    public class CreateContentViewModel
    {
        [Required(ErrorMessage = "Content Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Title { get; set; }

        public string? VideoUrl { get; set; }
        public string? ArticleUrl { get; set; }
        public string? Text { get; set; }

        public IFormFile? VideoFile { get; set; }
        public string ContentType { get; set; } // "video", "link", "text"
    }
}