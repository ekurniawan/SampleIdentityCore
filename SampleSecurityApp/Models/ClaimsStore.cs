using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SampleSecurityApp.Models
{
    public static class ClaimsStore
    {
        public static List<Claim> AllClaims = new List<Claim>
        {
            new Claim("Create User","Create User"),
            new Claim("Edit User","Edit User"),
            new Claim("Delete User","Delete User")
        };
    }
}
