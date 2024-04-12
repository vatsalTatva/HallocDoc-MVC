using DataAccess.CustomModels;
using DataAccess.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IPatientService
    {
        bool AddPatientInfo(PatientInfoModel patientInfoModel);

        bool IsEmailExists(string email);
        bool IsPasswordExists(string email);
        bool CreateAccount(CreateAccountModel model);

        bool AddFamilyReq(FamilyReqModel familyReqModel, string createAccountLink);

        bool AddConciergeReq(ConciergeReqModel conciergeReqModel, string createAccountLink);  

        bool AddBusinessReq(BusinessReqModel businessReqModel, string createAccountLink);


        MedicalHistoryList GetMedicalHistory(int userid);
        DocumentModel GetAllDocById(int requestId);
        Profile GetProfile(int userid);
        bool EditProfile(Profile profile);

        bool UploadDocuments(List<IFormFile> files, int reqId);
    }
}
