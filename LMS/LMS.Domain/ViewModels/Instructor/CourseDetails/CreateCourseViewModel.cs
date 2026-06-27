using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace LMS.Domain.ViewModels.Instructor.CourseDetails
{
    public class CreateCourseViewModel
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0, 100000, ErrorMessage = "Price must be a positive value.")]
        public decimal Price { get; set; }

        [Display(Name = "Thumbnail File")]
        public IFormFile? ThumbnailFile { get; set; }

        public string? ExistingThumbnailUrl { get; set; }
    }
}