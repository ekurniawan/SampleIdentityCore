using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SampleSecurityApp.ViewModels
{
    public class EditRoleViewModel
    {
        public string Id { get; set; }

        [Required]
        [Display(Name ="Role Name")]
        public string RoleName { get; set; }

        public List<string> Users { get; set; }
    }
}
