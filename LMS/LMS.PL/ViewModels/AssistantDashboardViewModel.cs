using System.Collections.Generic;

namespace LMS.PL.ViewModels
{
    public class AssistantDashboardViewModel
    {
        public int PendingSubmissionsCount { get; set; }
        public int GradedTodayCount { get; set; }
        public string? ActivePermissions { get; set; }
        public IEnumerable<SubmissionListItemViewModel>? RecentSubmissions { get; set; }
    }
}