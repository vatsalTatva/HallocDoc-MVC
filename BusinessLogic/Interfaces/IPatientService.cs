using DataAccess.CustomModels;
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

        List<PatientDashboard> GetPatientInfos();
        List<MedicalHistory> GetMedicalHistory(string email);
    }
}
