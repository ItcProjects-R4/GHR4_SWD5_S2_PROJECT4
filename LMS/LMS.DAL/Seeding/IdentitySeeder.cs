using LMS.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.DAL.Seeding
{
    public static class IdentitySeeder
    {
        public static async Task SeedIdentityAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles = {"Instructor", "Student", "Assistant"};

            foreach(var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var instructorEmail = "kk@gmail.com";

            if(await userManager.FindByEmailAsync(instructorEmail) == null)
            {
                var instructor = new ApplicationUser
                {
                    UserName = instructorEmail,
                    Email = instructorEmail,
                    FirstName = "Karima",
                    LastName = "Karim",
                };
                var res = await userManager.CreateAsync(instructor, "1234Aa##");

                if (res.Succeeded)
                {
                    await userManager.AddToRoleAsync(instructor, "Instructor");
                }
            }
        }
        }
}
