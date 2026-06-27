using LMS.Domain.ViewModels;

namespace LMS.BLL.Services.Interfaces
{
    public interface IAssistantService
    {
        Task<AssistantDashboardViewModel> GetDashboardAsync();
        Task<IEnumerable<SubmissionListItemViewModel>> GetSubmissionsAsync(string searchString, string statusFilter);
    }
}