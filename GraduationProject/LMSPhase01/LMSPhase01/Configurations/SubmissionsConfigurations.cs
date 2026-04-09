using LMSPhase01.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GHR4_SWD5_S2_PROJECT4.GraduationProject.LMSPhase01.LMSPhase01.Configurations
{
    public class SubmissionsConfigurations : IEntityTypeConfiguration<Submissions>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Submissions> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.SubmittedAt)
                .IsRequired();

            builder.HasOne(s => s.Assignment)
                .WithMany(a => a.Submissions)
                .HasForeignKey(s => s.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.User)
                .WithMany(u => u.Submissions)
                .HasForeignKey(s => s.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
