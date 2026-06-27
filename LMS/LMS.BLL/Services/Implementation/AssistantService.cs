using LMS.BLL.Services.Interfaces;
using LMS.Domain.Enums;
using LMS.Domain.ViewModels;

namespace LMS.BLL.Services.Implementation
{
    public class AssistantService : IAssistantService
    {
        private readonly ISubmissionService _submissionService;

        public AssistantService(ISubmissionService submissionService)
        {
            _submissionService = submissionService;
        }

        public async Task<AssistantDashboardViewModel> GetDashboardAsync()
        {
            var recentSubmissions = await _submissionService.GetRecentSubmissionsAsync(5);

            var recentViewModels = recentSubmissions.Select(s => new SubmissionListItemViewModel
            {
                Id = s.Id,
                StudentName = s.Student != null
                    ? $"{s.Student.FirstName} {s.Student.LastName}".Trim()
                    : "Unknown",

                StudentInitial = !string.IsNullOrEmpty(s.Student?.FirstName)
                    ? s.Student.FirstName.Substring(0, 1).ToUpper()
                    : "U",

                CourseTitle = s.Assignment?.Title ?? "Untitled Assignment",
                SubmittedAt = s.SubmittedAt,
                IsGraded = s.Status == SubmissionStatus.Graded,
                Grade = s.Grade ?? 0,
                Feedback = s.Comment ?? "No feedback provided."
            });

            return new AssistantDashboardViewModel
            {
                PendingSubmissionsCount = await _submissionService.GetPendingSubmissionsCountAsync(),
                GradedTodayCount = await _submissionService.GetGradedTodayCountAsync(),
                ActivePermissions = "",
                RecentSubmissions = recentViewModels
            };
        }

        public async Task<IEnumerable<SubmissionListItemViewModel>> GetSubmissionsAsync(string searchString, string statusFilter)
        {
            var submissions = await _submissionService.GetFilteredSubmissionsAsync(searchString, statusFilter);

            return submissions.Select(s => new SubmissionListItemViewModel
            {
                Id = s.Id,
                StudentName = s.Student != null
                    ? $"{s.Student.FirstName} {s.Student.LastName}".Trim()
                    : "Unknown",

                StudentInitial = !string.IsNullOrEmpty(s.Student?.FirstName)
                    ? s.Student.FirstName.Substring(0, 1).ToUpper()
                    : "U",

                CourseTitle = s.Assignment?.Title ?? "Untitled Assignment",
                SubmittedAt = s.SubmittedAt,
                IsGraded = s.Status == SubmissionStatus.Graded,
                Grade = s.Grade ?? 0,
                Feedback = s.Comment ?? "No feedback provided."
            });
        }
    }
}