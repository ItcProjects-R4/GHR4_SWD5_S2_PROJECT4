using System.ComponentModel.DataAnnotations;

namespace LMS.Domain.ViewModels
{
    public class AssistantSettingsViewModel
    {
        [Required(ErrorMessage = "First Name is required.")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public required string Email { get; set; }

        public string? AvatarUrl { get; set; }

        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string? ConfirmPassword { get; set; }
    }
}