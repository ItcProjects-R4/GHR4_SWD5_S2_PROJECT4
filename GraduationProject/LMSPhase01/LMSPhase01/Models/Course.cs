using System;
using System.Collections.Generic;
using System.Text;

namespace LMSPhase01.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ThumbnailUrl { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }

        // Navigation properties
        public int InstructorId { get; set; }
        public ApplicationUser Instructor { get; set; } = null!;
        
        public List<Module> Modules { get; set; }
        
        public List<Enrollment> Enrollments { get; set; }
    }
}
