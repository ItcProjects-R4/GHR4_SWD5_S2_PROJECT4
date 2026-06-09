using LMS.BLL.Services.Interfaces;
using LMS.Domain.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.BLL.Services.Implementation
{
    public class StudentService : IStudentService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IEnumerable<ApplicationUser>> GetFilteredUsersAsync(string searchString, string roleFilter)
        {
            IList<ApplicationUser> users;

            if (!string.IsNullOrEmpty(roleFilter) && !roleFilter.Equals("All Roles", StringComparison.OrdinalIgnoreCase))
            {
                users = await _userManager.GetUsersInRoleAsync(roleFilter);
            }
            else
            {
                var students = await _userManager.GetUsersInRoleAsync("Student");
                var assistants = await _userManager.GetUsersInRoleAsync("Assistant");
                users = students.Concat(assistants).ToList();
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                var query = searchString.ToLower();
                users = users.Where(u =>
                    (u.FirstName != null && u.FirstName.ToLower().Contains(query)) ||
                    (u.LastName != null && u.LastName.ToLower().Contains(query)) ||
                    (u.Email != null && u.Email.ToLower().Contains(query)) ||
                    u.Id.ToLower().Contains(query)
                ).ToList();
            }

            return users;
        }
    }
}