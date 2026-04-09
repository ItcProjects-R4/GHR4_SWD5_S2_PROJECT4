using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LMSPhase01.Models
{
    public class Progress
    {
        [Key]
        public int Id { get; set; }
      
        [Required]
        public string UserId { get; set; }

        [Required]
        public int LessonId { get; set; }

        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }

        // Navigations
        
        public ApplicationUser User { get; set; }
        public Lesson Lesson { get; set; }
    }
}
