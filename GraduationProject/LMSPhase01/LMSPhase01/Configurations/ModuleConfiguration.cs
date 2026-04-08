

using LMSPhase01.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMSPhase01.Configurations
{
    public class ModuleConfiguration : IEntityTypeConfiguration<Module>
    {
        public void Configure(EntityTypeBuilder<Module> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.CourseId)
                .IsRequired();

            builder.Property(m => m.Title)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.OrderIndex)
                .IsRequired();

            builder.HasIndex(m => new { m.CourseId, m.OrderIndex })
                .IsUnique();

            // Module → Course
            builder.HasOne(m => m.Course)
                .WithMany(c => c.Modules)
                .HasForeignKey(m => m.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Module → Lessons
            builder.HasMany(m => m.Lessons)
                .WithOne(l => l.Module)
                .HasForeignKey(l => l.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Module → Assignment (one-to-one)
            //builder.HasOne(m => m.Assignment)
            //    .WithOne(a => a.Module)
            //    .HasForeignKey<Assignment>(a => a.ModuleId)
            //    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
