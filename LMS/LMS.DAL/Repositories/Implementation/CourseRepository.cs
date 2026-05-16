using LMS.DAL.Data;
using LMS.DAL.Repositories.Interfaces;
using LMS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.DAL.Repositories.Implementation
{
    public class CourseRepository : ICourseRepository
    {
        private readonly ApplicationDbContext _context;
        public CourseRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Course> GetCourseByIdAsync(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (courseId == 0) {
                return null;
            }
            return course;
        }
    }
}
