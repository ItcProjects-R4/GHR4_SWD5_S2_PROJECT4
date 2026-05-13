using LMS.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace LMS.DAL.Configurations
{
        public class SubmissionFileConfiguration : IEntityTypeConfiguration<SubmissionFile>
        {
            public void Configure(EntityTypeBuilder<SubmissionFile> builder)
            {
                builder.HasOne(sf => sf.Submission)
                       .WithMany(s => s.SubmissionFiles)
                       .HasForeignKey(sf => sf.SubmissionId)
                       .OnDelete(DeleteBehavior.Restrict);
            }
        }
}