using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LMS.Domain.Models
{
    public class Content
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ModuleId { get; set; }

        [MaxLength(100)]
        public required string Title { get; set; } 

        [MaxLength(500)]
        [Url]
        public string? VideoUrl { get; set; }

        [MaxLength(500)]
        [Url]
        public string? ArticleUrl { get; set; }

        public string? Text { get; set; }

        [Required]
        public int OrderIndex { get; set; }

        // Navigations
        
        public Module Module { get; set; }

        public List<Progress> Progresses { get; set; }
    }
}
