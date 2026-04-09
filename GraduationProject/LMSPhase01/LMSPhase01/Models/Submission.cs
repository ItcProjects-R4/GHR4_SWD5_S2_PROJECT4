using LMSPhase01.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LMSPhase01.Models
{
    public class Submission
    {
        public int Id { get; set; }
        public DateTime SubmittedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? GradedAt { get; set; }
        public string? Link { get; set; }
        public string? Comment { get; set; }
        public int? Grade { get; set; }
        public SubmissionStatus Status { get; set; }
        public int AssignmentId { get; set; }
        [ForeignKey("AssignmentId")]
        public Assignment Assignment { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        public List<SubmissionFiles> SubmissionFiles { get; set; }
    }
}
