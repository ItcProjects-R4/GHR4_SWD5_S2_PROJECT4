using LMS.Domain.Models;
using LMS.BLL.ViewModels.Instructor.CourseDetails;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LMS.BLL.Services.Interfaces
{
    public interface IInstructorService
    {
        // ... (keep existing methods)

        Task<List<Course>> GetInstructorCoursesAsync(string instructorId, string? searchString, string? sortBy);
        Task<Course> CreateCourseAsync(CreateCourseViewModel model, string instructorId);
        Task<Course?> GetCourseForEditAsync(int courseId, string instructorId);
        Task<bool> UpdateCourseAsync(int courseId, CreateCourseViewModel model, string instructorId);
        Task<bool> DeleteCourseAsync(int courseId, string instructorId);
        Task<Module> AddModuleAsync(int courseId, string moduleTitle);
        Task<bool> DeleteModuleAsync(int moduleId, int courseId, string instructorId);
        Task<Content> AddContentAsync(int moduleId, CreateContentViewModel model);
        Task<bool> DeleteContentAsync(int contentId, int courseId, string instructorId);
        Task<Assignment> AddAssignmentAsync(int moduleId, string title, DateTime dueDate, int maxScore, IFormFile? resourceFile);
        Task<bool> DeleteAssignmentAsync(int assignmentId, int courseId, string instructorId);
        Task<List<Submission>> GetSubmissionsQueueAsync(string instructorId, string? searchString, string? statusFilter);
        Task<Submission?> GetSubmissionForGradingAsync(int submissionId, string instructorId);
        Task<bool> GradeSubmissionAsync(int submissionId, int grade, string? comment, string instructorId);
    }
}