using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LMSPhase01.Models
{
    public class Assignment
    {
        public int Id { get; set; }
        public string FileUrl { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public int MaxScore { get; set; }
        public int ModuleId { get; set; }

        [ForeignKey("ModuleId")]
        public Module Module { get; set; }
        public List<Submission> Submissions { get; set; }
    }
}
