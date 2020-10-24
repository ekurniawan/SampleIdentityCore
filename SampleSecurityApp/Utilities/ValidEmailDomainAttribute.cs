using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SampleSecurityApp.Utilities
{
    public class ValidEmailDomainAttribute : ValidationAttribute
    {
        private string _allowDomain;
        public ValidEmailDomainAttribute(string allowedDomain)
        {
            _allowDomain = allowedDomain;
        }
        public override bool IsValid(object value)
        {
            var email = value.ToString().Split('@');
            return email[1].ToUpper() == _allowDomain.ToUpper();
        }
    }
}
