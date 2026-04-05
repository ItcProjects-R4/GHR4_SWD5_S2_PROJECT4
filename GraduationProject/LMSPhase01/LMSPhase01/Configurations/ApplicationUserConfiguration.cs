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
                }


            );
        }
        
    }
}
