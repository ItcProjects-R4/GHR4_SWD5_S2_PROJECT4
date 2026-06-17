using LMS.Domain.Models.Results;
using LMS.Domain.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace LMS.BLL.Services.Interfaces
{
    public interface IAccountService
    {
        Task<IdentityResult> RegisterAsync(RegisterViewModel model);
        Task<LoginResult> LoginAsync(LoginViewModel model);
        Task LogoutAsync();
        Task ForgotPasswordAsync(ForgotPasswordViewModel model, string callbackUrl);
        Task<IdentityResult> ResetPasswordAsync(ResetPasswordViewModel model);

    }
}
