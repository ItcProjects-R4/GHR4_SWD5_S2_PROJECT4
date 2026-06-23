using LMS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.BLL.Services.Interfaces
{
    public interface ICourseService
    {
        Task<IEnumerable<Course>> GetFilteredCoursesAsync(string? searchString, string typeFilter, string sortOrder);
        Task<Course?> GetCourseByIdAsync(int id);
        Task<IEnumerable<Course>> GetFeaturedCoursesAsync(int count);
    }
}
