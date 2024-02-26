using DataAccess.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class PatientInfoModel
    {
        public string? symptoms { get; set; }

        [Required(ErrorMessage = "First name is required")]
        public string firstName { get; set; }
        public string? lastName { get; set; }
        public DateTime? dob { get; set; }

        [Required(ErrorMessage = "Please enter the patient's email address.")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Please enter a valid email address.")]
        public string email { get; set; }
        public string? phoneNo { get; set; }
        public string? street { get; set; }
        public string? city { get; set; }
        public string? state { get; set; }
        public string? zipCode { get; set; }
        public string? roomNo { get; set; }
        public string? country { get; set; }

        public string? password { get; set; }

        [Compare("password", ErrorMessage = "Password Missmatch")]
        public string? confirmPassword { get; set; }

        public List<IFormFile>? file { get; set; }


    }

    public class FamilyReqModel
    {
        [Required(ErrorMessage = "First name is required")]
        public string firstName { get; set; }
        public string? lastName { get; set; }
        public string? email { get; set; }
        public string? phoneNo { get; set; }

        [Required(ErrorMessage = "Please Enter Relation")]
        public string? relation { get; set; }
        public string? symptoms { get; set; }

        [Required(ErrorMessage = "Patient First name is required")]
        public string? patientFirstName { get; set; }
        public string? patientLastName { get; set; }
        public DateTime? patientDob { get; set; }

        [Required(ErrorMessage = "Please enter the patient's email address.")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Please enter a valid email address.")]
        public string? patientEmail { get; set; }
        public string? patientPhoneNo { get; set; }
        public string? street { get; set; }
        public string? city { get; set; }
        public string? state { get; set; }
        public string? zipCode { get; set; }
        public int? roomNo { get; set; }
        public List<IFormFile>? file { get; set; }
    }

    public class ConciergeReqModel
    {

        [Required(ErrorMessage = "First name is required")]
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? email { get; set; }
        public string? phoneNo { get; set; }

        [Required(ErrorMessage = "Please Enter Hotel/Property Name")]
        public string? hotelName { get; set; }
        public string? symptoms { get; set; }

        [Required(ErrorMessage = "Patient First name is required")]
        public string? patientFirstName { get; set; }
        public string? patientLastName { get; set; }
        public DateTime? patientDob { get; set; }

        [Required(ErrorMessage = "Please enter the patient's email address.")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Please enter a valid email address.")]
        public string? patientEmail { get; set; }
        public string? patientPhoneNo { get; set; }

        [Required(ErrorMessage = "Please Enter Street")]
        public string street { get; set; }
        [Required(ErrorMessage = "Please Enter City")]
        public string city { get; set; }
        [Required(ErrorMessage = "Please Enter State")]
        public string state { get; set; }
        [Required(ErrorMessage = "Please Enter ZipCode")]
        public string? zipCode { get; set; }
        public int? roomNo { get; set; }


    }

    public class BusinessReqModel
    {

        [Required(ErrorMessage = "First name is required")]
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? email { get; set; }
        public string? phoneNo { get; set; }

        [Required(ErrorMessage = "Please Enter Business/Property Name")]
        public string? businessName { get; set; }
        public string? caseNo { get; set; }
        public string? symptoms { get; set; }

        [Required(ErrorMessage = "Patient First name is required")]
        public string? patientFirstName { get; set; }
        public string? patientLastName { get; set; }
        public DateTime? patientDob { get; set; }

        [Required(ErrorMessage = "Please enter the patient's email address.")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Please enter a valid email address.")]
        public string? patientEmail { get; set; }
        public string? patientPhoneNo { get; set; }
        public string? street { get; set; }
        public string? city { get; set; }
        public string? state { get; set; }
        public string? zipCode { get; set; }
        public int? roomNo { get; set; }
    }

    

    public class MedicalHistory
    {
        
        public int redId { get; set; }
        public DateTime createdDate { get; set; }
        public int currentStatus { get; set; }
        public List<string> document { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public string? PhoneNo { get; set; }

        public string? Street { get; set; }

        public string? City { get; set; }

        public string? ZipCode { get; set; }

        public string? State { get; set; }

        public string? Email { get; set; }
    }
    public class MedicalHistoryList
    {
        public List<MedicalHistory> medicalHistoriesList { get; set; }

          
    }
}
