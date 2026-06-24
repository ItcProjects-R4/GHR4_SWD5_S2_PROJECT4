using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.ViewModels
{
    public class ProfileSettingsViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string AvatarUrl { get; set; }
        public string MemberSince { get; set; }
        public string? Biography { get; set; }

        public UpdateProfileViewModel ProfileInfo { get; set; }
        public UpdatePasswordViewModel PasswordInfo { get; set; }
    }
}
