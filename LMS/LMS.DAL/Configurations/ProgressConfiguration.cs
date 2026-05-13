using LMS.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.DAL.Configurations
{
    public class ProgressConfiguration : IEntityTypeConfiguration<Progress>
    {
        public void Configure(EntityTypeBuilder<Progress> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.StudentId)
                .IsRequired();

            builder.Property(p => p.ContentId)
                .IsRequired();

            builder.Property(p => p.IsCompleted)
                .IsRequired();

            builder.Property(p => p.CompletedAt)
                .IsRequired(false);

            builder.HasOne(p => p.Student)
                .WithMany()
                .HasForeignKey(p => p.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Content)
                .WithMany(c => c.Progresses)
                .HasForeignKey(p => p.ContentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => new { p.StudentId, p.ContentId })
                   .IsUnique();


        }
    }
}
