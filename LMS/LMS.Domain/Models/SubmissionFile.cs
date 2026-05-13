
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;



namespace LMS.Domain.Models
{
    public class SubmissionFile
    {
        public int Id { get; set; }  

        [Required]
        public int SubmissionId { get; set; }  

        [Required]
        public string FileUrl { get; set; }

        public string? FileName { get; set; }

        public string? FileType { get; set; }

        public double? FileSize { get; set; }

        // Navigation
        [ForeignKey("SubmissionId")]
        public Submission Submission { get; set; }
    }
}