using LMS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.Models
{
    public class Module
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public int OrderIndex { get; set; }

        // Navigation properties
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        public List<Content> Contents { get; set; }

        public Assignment Assignment { get; set; }
    }
}