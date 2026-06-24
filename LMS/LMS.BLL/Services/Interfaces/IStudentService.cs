using LMS.Domain.ViewModels.Student.CourseDetails;
using LMS.Domain.ViewModels.Student.Dashboard;
using LMS.Domain.Models;
using Microsoft.AspNetCore.Http;
using LMS.Domain.ViewModels;

namespace LMS.BLL.Services.Interfaces
{
    public interface IStudentService
    {
        Task<IEnumerable<ApplicationUser>> GetFilteredUsersAsync(string searchString, string roleFilter);

        Task<StudentDashboardViewModel> GetStudentDashboardAsync();
        Task<List<EnrolledCoursesViewModel>> GetEnrolledCoursesAsync(string status, string search);
        Task<List<BrowseCourseViewModel>> GetBrowseCoursesAsync();
        Task<CheckoutResponse> EnrollCourseAsync(int courseId);
        Task<CourseDetailsPageViewModel> GetCourseDetailsPageAsync(int courseId);
        Task<ContentViewModel> GetContentAsync(int contentId);
        Task<bool> MarkContentAsCompletedAsync(int contentId, int courseId);
        Task<AssignmentViewModel> GetAssignmentDetailsAsync(int assignmentId);
        Task<AssignmentViewModel> SubmitAssignmentAsync(int AssignmentID, List<IFormFile> submissionFiles);
    }
}