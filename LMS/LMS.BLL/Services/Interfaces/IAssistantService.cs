using LMS.Domain.ViewModels.Assistant;
using LMS.Domain.ViewModels.Shared;



namespace LMS.BLL.Services.Interfaces
{
    public interface IAssistantService
    {
        Task<AssistantDashboardViewModel> GetDashboardAsync();
        Task<IEnumerable<SubmissionListItemViewModel>> GetSubmissionsAsync(string searchString, string statusFilter);
    }
}