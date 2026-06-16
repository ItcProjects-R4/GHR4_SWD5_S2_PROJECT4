using AutoMapper;
using CloudinaryDotNet;
using LMS.Domain.Models;
using LMS.PL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using System.ComponentModel.DataAnnotations;

namespace LMS.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IMapper mapper, IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
            _emailSender = emailSender;
        }


        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dashboard", "Student");

            }
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> RegisterAsync(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
               
            // search for the user in the db by email
            var user = await _userManager.FindByEmailAsync(model.Email);
          
            if (user != null)
            {
                ModelState.AddModelError(string.Empty, "User with this email already exists.");
                return View(model);
            }
            // if not registered, add them
            var newUser = _mapper.Map<ApplicationUser>(model);
            newUser.UserName = model.Email;
            var res = await _userManager.CreateAsync(newUser, model.Password);
            if (res.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, "Student");
                await _signInManager.SignInAsync(newUser, isPersistent: false);
                return RedirectToAction("Dashboard", "Student");
            }
          
            foreach (var error in res.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);

        }
        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };

            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            //find email or username in the db

            var isEmailValid = new EmailAddressAttribute().IsValid(model.Username);

            var user = isEmailValid ? await _userManager.FindByEmailAsync(model.Username)
                : await _userManager.FindByNameAsync(model.Username);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email or password is incorrect");
                return View(model);
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!isPasswordValid)
            {
                ModelState.AddModelError(string.Empty, "Email or password is incorrect");
                return View(model);

            }

            var res = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);

            if (res.Succeeded)
            {
                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl)){
                    return Redirect(model.ReturnUrl);
                }


                var roles = await _userManager.GetRolesAsync(user);

                if(roles.Contains("Instructor")) return RedirectToAction("Dashboard", "Instructor");
                if(roles.Contains("Student")) return RedirectToAction("Dashboard", "Student");
                if(roles.Contains("Assistant")) return RedirectToAction("Dashboard", "Assistant");

            }
            ModelState.AddModelError(string.Empty, "Email or password is incorrect.");
            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return RedirectToAction("ForgotPasswordConfirmation");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = Url.Action("ResetPassword", "Account",
                new { token, email = user.Email }, Request.Scheme);

            await _emailSender.SendEmailAsync(
                user.Email,
                "Reset Your Password",
                $"<p>Click <a href='{resetLink}'>here</a> to reset your password.</p>"
            );

            return RedirectToAction("ForgotPasswordConfirmation");
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string token = null, string email = null)
        {
            if (token == null || email == null)
            {
                return RedirectToAction("Error", "Home");
            }
            var model = new ResetPasswordViewModel
            {
                Token = token,
                Email = email
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }
           
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

    }
}
