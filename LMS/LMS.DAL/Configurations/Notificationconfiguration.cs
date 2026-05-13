using LMS.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace LMS.DAL.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(n => n.Id);

            
            builder.Property(n => n.Title)
                .IsRequired()
                .HasMaxLength(200);

           
            builder.Property(n => n.Message)
                .IsRequired();

            
            builder.Property(n => n.IsRead)
                .HasDefaultValue(false)
                .IsRequired();

            
            builder.Property(n => n.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
