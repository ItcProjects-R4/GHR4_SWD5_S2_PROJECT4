using LMS.BLL.Services.Interfaces;
using LMS.DAL.Data;
using LMS.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.BLL.Services.Implementation
{
    public class CourseService : ICourseService
    {
        private readonly ApplicationDbContext _context;

        public CourseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Course>> GetFilteredCoursesAsync(string? searchString, string typeFilter, string sortOrder)
        {
            var query = _context.Courses.AsQueryable();

            // 1.  Filter 
            if (!string.IsNullOrEmpty(typeFilter) && !typeFilter.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                if (typeFilter.Equals("free", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(c => c.Price == 0);
                }
                else if (typeFilter.Equals("paid", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(c => c.Price > 0);
                }
            }

            // 2. Search 
            if (!string.IsNullOrEmpty(searchString))
            {
                var search = searchString.ToLower();
                query = query.Where(c =>
                    c.Title.ToLower().Contains(search) ||
                    c.Description.ToLower().Contains(search)
                );
            }

            // 3. Sorting 
            query = sortOrder switch
            {
                "title-asc" => query.OrderBy(c => c.Title),
                "price-asc" => query.OrderBy(c => c.Price),
                "price-desc" => query.OrderByDescending(c => c.Price),
                _ => query.OrderByDescending(c => c.Id) 
            };

            return await query.ToListAsync();
        }

        public async Task<Course?> GetCourseByIdAsync(int id)
        {
            return await _context.Courses
                .Include(c => c.Modules)
                    .ThenInclude(m => m.Contents) 
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Course>> GetFeaturedCoursesAsync(int count)
        {
            return await _context.Courses.Take(count).ToListAsync();
        }
    }
}
