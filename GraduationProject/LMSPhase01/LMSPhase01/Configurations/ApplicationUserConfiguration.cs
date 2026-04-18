using LMSPhase01.Constants;
using LMSPhase01.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMSPhase01.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {

            builder.HasData(
                new ApplicationUser
                {
                    Id = "1",
                    UserName = "karimakarim",
                    FirstName = "Karima",
                    LastName = "Karim",
                    Email = "kk@gmail.com",
                    PhoneNumber = "123456789",
                    Role = Roles.Instructor,
                    ConcurrencyStamp = "d3b07384-d9a3-4c5f-bb9a-123456789abc",
                    SecurityStamp = "a8f5f167-f44f-4b2c-9f6e-abcdef123456"
                });

            builder.HasMany(u => u.Courses)
                .WithOne(c => c.Instructor)
                .HasForeignKey(c => c.InstructorId);



            builder.HasMany(u => u.Enrollments)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId);


            builder.HasMany(u => u.Progresses)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId);



            builder.HasMany(u => u.Submissions)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserId);



            builder.HasMany(u => u.Notifications)
                .WithOne(n => n.User)
                .HasForeignKey(n => n.UserId);
               
        }
        
    }
}
