using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LMS.Domain.ViewModels.Home
{
    public class ContactFormViewModel
    {
        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Subject is required.")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Message cannot be empty.")]
        public string Message { get; set; } = string.Empty;
    }
}
