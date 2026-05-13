using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LMS.Domain.Models
{
    public class Progress
    {
        [Key]
        public int Id { get; set; }
      
        [Required]
        public required string StudentId { get; set; }

        [Required]
        public int ContentId { get; set; }

        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }

        // Navigations
        
        public ApplicationUser Student { get; set; }
        public Content Content { get; set; }
    }
}
