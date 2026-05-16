using LMS.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace LMS.DAL.Configurations
{
    public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            builder.HasOne(e => e.Student)
                           .WithMany(u => u.Enrollments)
                           .HasForeignKey(e => e.StudentId)
                           .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(e => e.Course)
                   .WithMany(c => c.Enrollments)
                   .HasForeignKey(e => e.CourseId)
                   .OnDelete(DeleteBehavior.Restrict);

           builder.HasOne(e => e.Payment)
                   .WithOne(p => p.Enrollment)
                   .HasForeignKey<Payment>(p => p.EnrollmentId)
                   .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
