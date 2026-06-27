using LMS.Domain.Models.Results;
using LMS.Domain.ViewModels.Account;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace LMS.BLL.Services.Interfaces
{
    public interface IAccountService
    {
        //account services
        Task<IdentityResult> RegisterAsync(RegisterViewModel model);
        Task<LoginResult> LoginAsync(LoginViewModel model);
        Task LogoutAsync();
        Task ForgotPasswordAsync(ForgotPasswordViewModel model, string callbackUrl);
        Task<IdentityResult> ResetPasswordAsync(ResetPasswordViewModel model);

        //settings services
        Task<ProfileSettingsViewModel> GetProfileSettingsAsync(string userId);
        Task<IdentityResult> UpdateProfileAsync(string userId, UpdateProfileViewModel model);
        Task<IdentityResult> UpdatePasswordAsync(string userId, UpdatePasswordViewModel model);
        Task<string> UpdateAvatarAsync(string userId, IFormFile avatarFile);


    }
}
