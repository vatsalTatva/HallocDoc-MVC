using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class AdminLoginModel
    {
        [Required(ErrorMessage = "Email is required")]
        public string email {  get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string password { get; set; }
    }

    public class AdminDashTableModel {
        public int status { get; set; }
        public string? name { get; set; }
        public DateTime? dateOfBirth { get; set; }
        public string? requestor { get; set; }
        public DateTime? requestedDate { get; set; }
        public string? phone { get; set; }
        public string? address { get; set; }
        public string? notes { get; set; }

        public string? chatsWith { get; set; }

        public string? actions { get; set; }
    }
}
