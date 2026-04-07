using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LMSPhase01.Models
{
    public class Lesson
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Module")]
        public int ModuleId { get; set; }

        [MaxLength(100)]
        public string Title { get; set; } 

        [MaxLength(500)]
        [Url]
        public string? VideoUrl { get; set; }

        [Required]
        public int OrderIndex { get; set; }

        // Navigations
        [ForeignKey(nameof(ModuleId))]
        public Module Module { get; set; }

        public List<Progress> Progresses { get; set; }
    }
}
