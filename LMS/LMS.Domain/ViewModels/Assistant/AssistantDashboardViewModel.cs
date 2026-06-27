using LMS.Domain.ViewModels.Shared;
using System.Collections.Generic;

namespace LMS.Domain.ViewModels.Assistant
{
    public class AssistantDashboardViewModel
    {
        public int PendingSubmissionsCount { get; set; }
        public int GradedTodayCount { get; set; }
        public string? ActivePermissions { get; set; }
        public IEnumerable<SubmissionListItemViewModel>? RecentSubmissions { get; set; }
    }
}