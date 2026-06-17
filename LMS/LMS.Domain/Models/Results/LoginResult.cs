using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.Models.Results
{
    public class LoginResult
    {
        public bool Succeeded { get; set; }
        public string Role { get; set; }
        public string ErrorMessage { get; set; }    
    }
}
