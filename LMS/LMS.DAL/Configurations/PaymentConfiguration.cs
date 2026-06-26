using LMS.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace LMS.DAL.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(p => p.Id);


            builder.Property(p => p.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");


            builder.Property(p => p.Status)
                .IsRequired();


            builder.Property(p => p.TransactionId)
                .HasMaxLength(100);


            builder.Property(p => p.PaidAt)
                .IsRequired();


            builder.HasOne(p => p.Enrollment)
                .WithOne(e => e.Payment)
                .HasForeignKey<Payment>(p => p.EnrollmentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(p => p.Student)
                   .WithMany(s => s.Payments)
                   .HasForeignKey(p => p.StudentId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Course)
                    .WithMany(c => c.Payments)
                    .HasForeignKey(p => p.CourseId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
