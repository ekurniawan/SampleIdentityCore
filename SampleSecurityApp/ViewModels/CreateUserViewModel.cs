using Microsoft.AspNetCore.Mvc;
using SampleSecurityApp.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SampleSecurityApp.ViewModels
{
    public class CreateUserViewModel
    {
        [Required]
        [EmailAddress]
        [Remote(action: "CheckEmailInUse", controller: "Account")]
        public string Email { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }
    }
}
