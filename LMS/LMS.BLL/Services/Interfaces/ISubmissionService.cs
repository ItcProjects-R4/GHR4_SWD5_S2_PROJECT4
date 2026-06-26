using LMS.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LMS.BLL.Services.Interfaces
{
    public interface ISubmissionService
    {
        Task<IEnumerable<Submission>> GetFilteredSubmissionsAsync(string searchString, string statusFilter);
        Task<int> GetPendingSubmissionsCountAsync();
        Task<int> GetGradedTodayCountAsync();
        Task<IEnumerable<Submission>> GetRecentSubmissionsAsync(int count);
        Task<Submission> GetSubmissionByIdAsync(int id);
        Task<bool> GradeSubmissionAsync(int id, int grade, string feedback);
    }
}