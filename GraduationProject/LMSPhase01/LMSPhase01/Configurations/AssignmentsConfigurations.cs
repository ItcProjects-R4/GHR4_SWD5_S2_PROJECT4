using LMSPhase01.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GHR4_SWD5_S2_PROJECT4.GraduationProject.LMSPhase01.LMSPhase01.Configurations
{
    public class AssignmentsConfigurations : IEntityTypeConfiguration<Assignments>
    {
        public void Configure(EntityTypeBuilder<Assignments> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Title)
                .IsRequired()
                .HasMaxLength(100);


            builder.Property(a => a.FileUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(a => a.CreatedAt)
                .IsRequired();

            builder.Property(a => a.MaxScore)
                .IsRequired();

            builder.HasOne(a => a.Module)
                .WithOne(m => m.Assignments)
                .HasForeignKey(a => a.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
