using AutoMapper;
using LMS.BLL.Services.Interfaces;
using LMS.Domain.Models;
using LMS.Domain.Models.Results;
using LMS.Domain.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.ComponentModel.DataAnnotations;

namespace LMS.BLL.Services.Implementation
{
    public class AccountService : IAccountService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;

        public AccountService(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IEmailSender emailSender
            )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
            _emailSender = emailSender;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterViewModel model)
        {
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "Email already exists."
                });

            var newUser = _mapper.Map<ApplicationUser>(model);
            newUser.UserName = model.Email;

            var result = await _userManager.CreateAsync(newUser, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, "Student");
                await _signInManager.SignInAsync(newUser, isPersistent: false);
            }

            return result;
        }
        public async Task<LoginResult> LoginAsync(LoginViewModel model)
        {

            var isEmailValid = new EmailAddressAttribute().IsValid(model.Username);

            var user = isEmailValid
                ? await _userManager.FindByEmailAsync(model.Username)
                : await _userManager.FindByNameAsync(model.Username);

            if (user == null)
            {
                return new LoginResult { Succeeded = false, ErrorMessage = "Email or password is incorrect." };
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!isPasswordValid)
            {
                return new LoginResult { Succeeded = false, ErrorMessage = "Email or password is incorrect." };
            }

            var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);

            if (!signInResult.Succeeded)
            {
                return new LoginResult { Succeeded = false, ErrorMessage = "Email or password is incorrect." };
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.Contains("Instructor") ? "Instructor"
                     : roles.Contains("Student") ? "Student"
                     : roles.Contains("Assistant") ? "Assistant"
                     : null;

            return new LoginResult { Succeeded = true, Role = role };


        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }


        public async Task ForgotPasswordAsync(ForgotPasswordViewModel model, string resetLink)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // rebuild the link with the token
            var tokenLink = resetLink + $"&token={Uri.EscapeDataString(token)}";

            await _emailSender.SendEmailAsync(
                user.Email,
                "Reset Your Password",
                $"<p>Click <a href='{tokenLink}'>here</a> to reset your password.</p>"
            );
        }
       
        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return IdentityResult.Success; // don't reveal user existence

            return await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
        }
    }
}
