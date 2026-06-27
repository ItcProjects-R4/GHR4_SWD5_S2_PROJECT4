using LMS.Domain.ViewModels.Shared;
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
