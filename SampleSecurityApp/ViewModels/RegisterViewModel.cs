using Microsoft.AspNetCore.Mvc;
using SampleSecurityApp.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SampleSecurityApp.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Remote(action: "CheckEmailInUse",controller:"Account")]
        [ValidEmailDomain(allowedDomain:"actual-training.com",
            ErrorMessage ="Domain harus actual training")]
        public string Email { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name="Confirmation Password")]
        [Compare("Password",ErrorMessage ="Password dan Confirm Password tidak sama")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }
    }
}
