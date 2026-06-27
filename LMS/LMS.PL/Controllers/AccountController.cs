using LMS.BLL.Services.Interfaces;
using LMS.Domain.ViewModels.Account;
using Microsoft.AspNetCore.Mvc;

namespace LMS.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _service;
 

        public AccountController(IAccountService service)
        {
            _service = service;
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
            if (!ModelState.IsValid) return View(model);

            var result = await _service.RegisterAsync(model);
            if (result.Succeeded)
            {
                return RedirectToAction("Dashboard", "Student");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);

        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                
                if (User.IsInRole("Instructor"))
                {
                    return RedirectToAction("Dashboard", "Instructor");
                }
                if (User.IsInRole("Assistant"))
                {
                    return RedirectToAction("Dashboard", "Assistant");
                }
                return RedirectToAction("Dashboard", "Student");
            }
            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };
            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _service.LoginAsync(model);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }
                return result.Role switch
                {
                    "Instructor" => RedirectToAction("Dashboard", "Instructor"),
                    "Student" => RedirectToAction("Dashboard", "Student"),
                    "Assistant" => RedirectToAction("Dashboard", "Assistant"),
                    _ => RedirectToAction("Index", "Home")
                };
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _service.LogoutAsync();
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

            var resetLink = Url.Action("ResetPassword", "Account", new { email = model.Email }, Request.Scheme);

            await _service.ForgotPasswordAsync(model, resetLink);

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
                return RedirectToAction("Error", "Home");

            return View(new ResetPasswordViewModel { Token = token, Email = email });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _service.ResetPasswordAsync(model);
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

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
