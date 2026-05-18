using System.ComponentModel.DataAnnotations;

namespace LMS.PL.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter your email or username")]
        public string Username { get; set; }
       
        [Required(ErrorMessage = "Please enter your password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
