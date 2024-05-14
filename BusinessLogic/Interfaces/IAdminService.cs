
using DataAccess.CustomModels;
using DataAccess.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IAdminService
    {
        LoginDetail GetLoginDetail(string email);
        Aspnetuser GetAspnetuser(string email);
        DashboardModel GetRequestsByStatus(int status, int CurrentPage);
        List<AdminDashTableModel> Export(int tabNo);
        DashboardModel GetRequestByRegion(FilterModel filterModel);

        StatusCountModel GetStatusCount();

        ViewCaseViewModel ViewCaseViewModel(int Requestclientid,int RequestTypeId);

        ViewNotesModel ViewNotes(int ReqId);

        bool UpdateAdminNotes(string additionalNotes, int reqId,int aspNetRole);

        CancelCaseModel CancelCase(int reqId);

        bool SubmitCancelCase(CancelCaseModel cancelCaseModel);

        AssignCaseModel AssignCase(int reqId);

        List<Physician> GetPhysicianByRegion(int Regionid);
        bool SubmitAssignCase(AssignCaseModel assignCaseModel);
        BlockCaseModel BlockCase(int reqId);
        bool SubmitBlockCase(BlockCaseModel blockCaseModel);
        ViewUploadModel GetAllDocById(int requestId);

        bool UploadFiles(List<IFormFile> files, int reqId);

        bool DeleteFileById(int reqFileId);

        bool DeleteAllFiles(List<string> filename ,  int reqId);
        Order FetchProfession();
        JsonArray FetchVendors(int selectedValue);
        Healthprofessional VendorDetails(int selectedValue);
        bool SendOrder(Order order);
        bool ClearCase(int reqId);
        SendAgreementModel SendAgreementCase(int reqId);
        CloseCaseModel ShowCloseCase(int reqId);

        bool SaveCloseCase(CloseCaseModel closeCaseModel);
        bool SubmitCloseCase(int ReqId);
        EncounterFormModel EncounterForm(int reqId);

        bool SubmitEncounterForm(EncounterFormModel encounterFormModel);

        bool AgreeAgreement(AgreementModel model);

        AgreementModel CancelAgreement(int reqId);

        bool SubmitCancelAgreement(AgreementModel model);
        int GetStatusForReviewAgreement(int reqId);

        MyProfileModel MyProfile(string email);
        bool ResetPassword(string tokenEmail, string resetPassword);
        bool SubmitAdminInfo(MyProfileModel model, string tokenEmail);
        bool SubmitBillingInfo(MyProfileModel model, string tokenEmail);
        bool VerifyState(string state);

        bool CreateRequest(CreateRequestModel model, string sessionEmail,string createAccountLink);

        List<ProviderModel> GetProvider();
        List<ProviderModel> GetProviderByRegion(int regionId);

        //ProviderModel ProviderContact(int phyId);
        public bool StopNotification(int phyId);

        bool ProviderContactEmail(int phyIdMain, string msg, string tokenEmail);
        bool ProviderContactSms(int phyId, string msg, string tokenEmail);
        CreateProviderAccount CreateProviderAccount(CreateProviderAccount obj, List<int> physicianRegions);
  
        EditProviderModel EditProviderProfile(int phyId, string tokenEmail);
        List<Region> RegionTable();
        List<PhysicianRegionTable> PhyRegionTable(int phyId);
        List<AdminRegionTable> AdminRegionTable(string email);
        List<AdminRegionTable> AdminRegionTableById(int adminid);
        List<AccountAccess> AccountAccess();
        bool DeleteRole(int roleId);
        List<AccountMenu> GetAccountMenu(int accounttype, int roleid);
        AccountAccess GetEditAccessData(int roleid);
        bool SetEditAccessAccount(AccountAccess accountAccess, List<int> AccountMenu, string sessionEmail);
        List<Aspnetrole> GetAccountType();
        CreateAccess FetchRole(short selectedValue);
        bool CreateRole(List<int> menuIds, string roleName, short accountType);

        bool RoleExists(string roleName, short accountType);
        List<Physicianlocation> GetPhysicianlocations();
        List<Role> GetPhyRoles();
        List<Role> GetAdminRoles();

        bool providerResetPass(string email, string password);
        bool editProviderForm1(int phyId,int roleId,int statusId);
        bool editProviderForm2(string fname, string lname, string email, string phone, string medical, string npi, string sync, int phyId, int[] phyRegionArray);
        bool editProviderForm3(EditProviderModel2 dataMain);
        bool PhysicianBusinessInfoUpdate(EditProviderModel2 dataMain);
        bool EditOnBoardingData(EditProviderModel2 dataMain);
        void editProviderDeleteAccount(int phyId);
        bool CreateAdminAccount(CreateAdminAccount obj, List<int> AdminRegion, string email);
        void CreateNewShiftSubmit(string selectedDays, CreateShiftModel obj, int adminId);

        List<RequestsRecordModel> SearchRecords(RecordsModel recordsModel);
        public bool DeleteRecords(int reqId);
        byte[] GenerateExcelFile(List<RequestsRecordModel> recordsModel);
        PatientRecordsModel PatientRecords(PatientRecordsModel patientRecordsModel,int currentPage);
        List<GetRecordExplore> GetPatientRecordExplore(int userId);
        List<BusinessTableModel> BusinessTable(string vendor, string profession);
        List<Healthprofessionaltype> GetProfession();
        bool AddBusiness(AddBusinessModel obj);
        bool RemoveBusiness(int VendorId);
        AddBusinessModel GetEditBusiness(int VendorId);
        List<UserAccess> FetchAccess(short selectedValue);
        CreateAdminAccount adminEditPage(int adminId);
        bool EditAdminDetailsDb(CreateAdminAccount model, string email, List<int> adminRegions);
        EmailSmsRecords2 EmailSmsLogs(int tempId, EmailSmsRecords2 recordsModel);

        List<BlockHistory> BlockHistory(BlockHistory2 blockHistory2);
        bool UnblockRequest(int blockId);
        bool IsBlockRequestActive(int blockId);

        DayWiseScheduling GetDayTable(string PartialName, string date, int regionid, int status);
        WeekWiseScheduling GetWeekTable(string date, int regionid, int status);
        MonthWiseScheduling GetMonthTable(string date, int regionid, int status);

        bool CreateShift(SchedulingViewModel model, string Email, List<int> repeatdays);

        CreateNewShift ViewShift(int ShiftDetailId);
        bool EditShift(CreateNewShift model, string email);
        bool ReturnShift(int ShiftDetailId, string email);
        bool DeleteShift(int ShiftDetailId, string email);

        OnCallModal GetOnCallDetails(int regionId);
        List<ShiftReview> GetShiftReview(int regionId, int callId);
        bool DeleteShiftReview(int[] shiftDetailsId, string Aspid);
        bool ApproveSelectedShift(int[] shiftDetailsId, string Aspid);


        List<PhysicianViewModel> GetPhysiciansForInvoicing();
        string CheckInvoicingApprove(string selectedValue, int PhysicianId);
        InvoicingViewModel GetApprovedViewData(string selectedValue, int PhysicianId);
        public void ApproveTimeSheet(InvoicingViewModel model, int? AdminID);
        GetPayRate GetPayRate(int physicianId, int callid);

        ChatViewModel GetChats(int RequestId, int AdminID, int ProviderId, int RoleId);
    }
}
