using LMS.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LMS.BLL.Services.Interfaces
{
    public interface IStudentService
    {
        Task<IEnumerable<ApplicationUser>> GetFilteredUsersAsync(string searchString, string roleFilter);
    }
}