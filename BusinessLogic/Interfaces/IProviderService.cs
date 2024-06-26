﻿using DataAccess.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IProviderService
    {
        LoginDetail GetLoginDetail(string email);

        bool TransferRequest(TransferRequest model);
        MonthWiseScheduling PhysicianMonthlySchedule(string date, int status, string aspnetuserid);
        int GetPhysicianId(string userid);
        void AcceptCase(int requestId, string loginUserId);
        void CallType(int requestId, short callType);
        public DashboardModel GetRequestsByStatus(int tabNo, int CurrentPage, int phyid);
        DashboardModel GetRequestByRegion(FilterModel filterModel,int phyid);
        public StatusCountModel GetStatusCount(int phyid);
        void housecall(int requestId);
        bool finalizesubmit(int reqid);
        bool concludecaresubmit(int ReqId, string ProviderNote);
        void RequestAdmin(RequestAdmin model, string sessionEmail);

        public List<DateViewModel> GetDates();
        InvoicingViewModel GetInvoicingDataonChangeOfDate(DateOnly startDate, DateOnly endDate, int? PhysicianId, int? AdminID);
        InvoicingViewModel GetUploadedDataonChangeOfDate(DateOnly startDate, DateOnly endDate, int? PhysicianId, int pageNumber, int pagesize);
        InvoicingViewModel getDataOfTimesheet(DateOnly startDate, DateOnly endDate, int? PhysicianId, int? AdminID);
        void AprooveTimeSheet(InvoicingViewModel model, int? AdminID);
        void SubmitTimeSheet(InvoicingViewModel model, int? PhysicianId);
        void DeleteBill(int id);
        void FinalizeTimeSheet(int id);
    }
}
