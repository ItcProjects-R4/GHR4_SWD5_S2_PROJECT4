using System;
using System.Collections.Generic;
using System.Text;

namespace LMSPhase01.Models
{
    public class Module
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int OrderIndex { get; set; }

        // Navigation properties
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
        
        public List<Lesson> Lessons { get; set; }
        
        public Assignment Assignment { get; set; }
    }
}