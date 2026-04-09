using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LMSPhase01.Models
{
    public class Submissions
    {
        public int Id { get; set; }
        public DateTimeOffset SubmittedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? GradedAt { get; set; }
        public string? link { get; set; }
        public string? comment { get; set; }
        public int? grade { get; set; }
        public enum SubmissionStatus
        {
            NotSubmitted,
            Submitted,
            Graded
        }
        public SubmissionStatus Status { get; set; }
        public int AssignmentId { get; set; }
        [ForeignKey("AssignmentId")]
        public Assignments Assignment { get; set; }
        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public ApplicationUser User { get; set; }
        public List<SubmissionFiles> SubmissionFiles { get; set; }
    }
}
