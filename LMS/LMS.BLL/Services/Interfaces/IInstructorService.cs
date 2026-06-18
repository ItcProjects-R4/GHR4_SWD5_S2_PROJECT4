using LMS.BLL.ViewModels.Instructor.Enrollments;
using LMS.BLL.ViewModels.Instructor.CourseDetails;
using LMS.BLL.ViewModels.Student.CourseDetails;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LMS.BLL.Services.Interfaces
{
    public interface IInstructorService
    {
        Task<List<CourseEnrollmentGroupViewModel>> GetEnrollmentsAsync(string search);
        Task<InstructorCourseDetailsPageViewModel> GetCourseDetailsPageAsync(int courseId);
        Task<ContentViewModel> GetContentAsync(int contentId);
        Task<InstructorAssignmentDetailsViewModel> GetAssignmentDetailsAsync(int assignmentId);
    }
}