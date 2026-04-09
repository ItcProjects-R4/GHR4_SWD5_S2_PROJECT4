using LMSPhase01.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GHR4_SWD5_S2_PROJECT4.GraduationProject.LMSPhase01.LMSPhase01.Configurations
{
    public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
    {
        public void Configure(EntityTypeBuilder<Assignment> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Title)
                .IsRequired()
                .HasMaxLength(100);


            builder.Property(a => a.FileUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(a => a.DueDate)
                .IsRequired();

            builder.Property(a => a.MaxScore)
                .IsRequired();

            builder.HasOne(a => a.Module)
                .WithOne(m => m.Assignment)
                .HasForeignKey<Assignment>(a => a.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
