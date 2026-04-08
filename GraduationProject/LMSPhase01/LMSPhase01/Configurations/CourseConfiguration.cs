using LMSPhase01.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace LMSPhase01.Configurations
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.InstructorId)
                .IsRequired();

            builder.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.ThumbnailUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(c => c.Description)
              .IsRequired();

            builder.Property(c => c.Price)
                .IsRequired();

            // Instructor → Courses
            builder.HasOne(c => c.Instructor)
                .WithMany(u => u.Courses)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Course → Modules
            builder.HasMany(c => c.Modules)
                .WithOne(m => m.Course)
                .HasForeignKey(m => m.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Course → Enrollments
            //builder.HasMany(c => c.Enrollments)
            //    .WithOne(e => e.Course)
            //    .HasForeignKey(e => e.CourseId)
            //    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
