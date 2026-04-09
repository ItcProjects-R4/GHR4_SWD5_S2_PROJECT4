using LMSPhase01.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMSPhase01.Configurations
{
        public class SubmissionFileConfiguration : IEntityTypeConfiguration<SubmissionFile>
        {
            public void Configure(EntityTypeBuilder<SubmissionFile> builder)
            {
                builder.HasOne(sf => sf.Submission)
                       .WithMany(s => s.SubmissionFiles)
                       .HasForeignKey(sf => sf.SubmissionId)
                       .OnDelete(DeleteBehavior.Cascade);
            }
        }
}