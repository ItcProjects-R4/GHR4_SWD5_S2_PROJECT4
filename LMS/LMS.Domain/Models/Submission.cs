using LMS.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;


namespace LMS.Domain.Models
{
    public class Submission
    {
        public int Id { get; set; }
        public DateTime SubmittedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? Link { get; set; }
        public string? Comment { get; set; }
        public int? Grade { get; set; }
        public SubmissionStatus Status { get; set; }
        public string StudentId { get; set; }
        public int AssignmentId { get; set; }
        
        [ForeignKey("StudentId")]
        public ApplicationUser Student { get; set; }

        [ForeignKey("AssignmentId")]
        public Assignment Assignment { get; set; }

        public List<SubmissionFile> SubmissionFiles { get; set; }
    }
}
