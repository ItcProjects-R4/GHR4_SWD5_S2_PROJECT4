using LMS.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.DAL.Repositories.Interfaces
{
    public interface ICourseRepository
    {
        Task<CourseViewModel> GetCourseByIdAsync(int courseId);
    }
}
