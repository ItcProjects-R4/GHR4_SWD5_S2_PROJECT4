using LMS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.DAL.Repositories.Interfaces
{
    public interface ICourseRepository
    {
        Task<Course> GetCourseByIdAsync(int courseId);
    }
}
