﻿using DataAccess.Models;
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
        public string email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string password { get; set; }
    }

    public class StatusCountModel
    {
        public int NewCount { get; set; }
        public int PendingCount { get; set; }
        public int ActiveCount { get; set; }
        public int ConcludeCount { get; set; }
        public int ToCloseCount { get; set; }
        public int UnpaidCount { get; set; }

    }

    public class AdminDashTableModel {
        public string? firstName { get; set; }

        public string? lastName { get; set; }

        public string? strMonth { get; set; }
        public int? intYear { get; set; }
        public int? intDate { get; set; }

        public string? requestorFname { get; set; }

        public string? requestorLname { get; set; }

        public DateTime createdDate { get; set; }

        public string? mobileNo { get; set; }

        public string? city { get; set; }

        public string? street { get; set; }

        public string? zipCode { get; set; }

        public string? state { get; set; }

        public string? notes { get; set; }

        public int? requestTypeId { get; set; }

        public int? status { get; set; }

        public int? requestClientId { get; set; }

        public int? reqId { get; set; }
    }

    public class ViewCaseViewModel
    {
        public int Requestclientid { get; set; }
        public int? RequestTypeId { get; set; }
        public int? Requestid { get; set; }
        public string Firstname { get; set; } = null!;
        public string? Lastname { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Phonenumber { get; set; }
        public string? Address { get; set; }
        public int? Regionid { get; set; }
        public string? Notes { get; set; }
        public string? Email { get; set; }
        public string? Strmonth { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Zipcode { get; set; }
        public string? Room { get; set; }
        public string? ConfirmationNumber { get; set; }
    }

    public class ViewNotesModel
    {
        public string? TransferNotes { get; set; }
        public string? PhysicianNotes { get; set; }

        public string? AdminNotes { get; set; }
        public string? AdditionalNotes { get; set; }

        public int ReqId { get; set; }
    }

    public class CancelCaseModel
    {
        public string? PatientFName { get; set; }
        public string? PatientLName { get; set; }
        public List<Casetag>? casetaglist { get; set; }

      
        public int? casetag { get; set; }
        public int? reqId { get; set; }
        public string? notes { get; set; }
    }
}

