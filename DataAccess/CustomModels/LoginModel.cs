using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Email is required")]
        public string? email { get; set; } = null;

        [Required(ErrorMessage = "Password is required")]
        public string? password { get; set; } = null;
    }
}
