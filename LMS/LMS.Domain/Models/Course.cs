using LMS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.Models
{
    public class Course
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? ThumbnailPublicId { get; set; }
        public required string ThumbnailUrl { get; set; }
        public required decimal Price { get; set; }
        public required string Description { get; set; }

        // Navigation properties
        public required string InstructorId { get; set; }
        public required ApplicationUser Instructor { get; set; }

        public List<Module> Modules { get; set; }

        public List<Enrollment> Enrollments { get; set; }
    }
}
