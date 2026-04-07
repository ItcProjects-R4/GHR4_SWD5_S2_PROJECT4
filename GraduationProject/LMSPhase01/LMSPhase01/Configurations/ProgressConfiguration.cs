using LMSPhase01.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GHR4_SWD5_S2_PROJECT4.GraduationProject.LMSPhase01.LMSPhase01.Configurations
{
    public class ProgressConfiguration : IEntityTypeConfiguration<Progress>
    {
        public void Configure(EntityTypeBuilder<Progress> builder)
        {
            builder.HasIndex(p => new { p.UserId, p.LessonId })
                   .IsUnique();
        }
    }
}
