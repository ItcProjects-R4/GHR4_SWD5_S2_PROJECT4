using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace LMSPhase01.Models
{
    public class SubmissionFiles
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
        [ForeignKey(nameof(SubmissionId))]
        public Submission Submission { get; set; }
    }
}
