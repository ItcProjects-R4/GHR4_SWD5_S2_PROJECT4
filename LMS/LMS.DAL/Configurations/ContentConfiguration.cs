
using LMS.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace LMS.DAL.Configurations
{
    public class ContentConfiguration : IEntityTypeConfiguration<Content>
    {
        public void Configure(EntityTypeBuilder<Content> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.VideoUrl)
                .HasMaxLength(500);

            builder.Property(c => c.ArticleUrl)
                .HasMaxLength(500);

            builder.Property(c => c.Text);

            builder.Property(c => c.OrderIndex)
                .IsRequired();

           
            builder.HasOne(c => c.Module)
                .WithMany(m => m.Contents)
                .HasForeignKey(c => c.ModuleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Progresses)
                .WithOne(p => p.Content)
                .HasForeignKey(p => p.ContentId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasIndex(l => new { l.ModuleId, l.OrderIndex })
                   .IsUnique();

                 
        }
    }
}
