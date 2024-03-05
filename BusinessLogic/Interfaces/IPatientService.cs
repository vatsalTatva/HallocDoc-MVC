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
        void AddPatientInfo(PatientInfoModel patientInfoModel);

        Task<bool> IsEmailExists(string email);  

        void AddFamilyReq(FamilyReqModel familyReqModel);

        void AddConciergeReq(ConciergeReqModel conciergeReqModel);  

        void AddBusinessReq(BusinessReqModel businessReqModel);


        MedicalHistoryList GetMedicalHistory(int userid);
        DocumentModel GetAllDocById(int requestId);
        Profile GetProfile(int userid);
        bool EditProfile(Profile profile);

        bool UploadDocuments(List<IFormFile> files, int reqId);
    }
}
