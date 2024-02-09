using BusinessLogic.Interfaces;
using DataAccess.CustomModels;
using DataAccess.Data;
using DataAccess.Models;
using System.Collections;

namespace BusinessLogic.Services
{
    public class PatientService : IPatientService
    {
        private readonly ApplicationDbContext _db;

        public PatientService(ApplicationDbContext db)
        {
            _db = db;
        }


        public void AddPatientInfo(PatientInfoModel patientInfoModel)
        {
            Request request = new Request();
            request.Requesttypeid = 2;
            request.Status = 1;
            request.Createddate = DateTime.Now;
            request.Isurgentemailsent = new BitArray(1);
            request.Firstname = patientInfoModel.firstName;
            request.Lastname = patientInfoModel.lastName;
            request.Phonenumber = patientInfoModel.phoneNo;
            request.Email = patientInfoModel.email;
            
            _db.Requests.Add(request);
            _db.SaveChanges();

            Requestclient info = new Requestclient();
            info.Requestid = request.Requestid;
            info.Notes = patientInfoModel.symptoms;
            info.Firstname = patientInfoModel.firstName;
            info.Lastname = patientInfoModel.lastName;
            info.Phonenumber = patientInfoModel.phoneNo;
            info.Email = patientInfoModel.email; 
            info.Street = patientInfoModel.street;
            info.City = patientInfoModel.city;
            info.State = patientInfoModel.state;
            info.Zipcode = patientInfoModel.zipCode;
            

            _db.Requestclients.Add(info);
            _db.SaveChanges();


            var user = _db.Aspnetusers.Where(x => x.Email == patientInfoModel.email).FirstOrDefault();
           


            User u = new User();
            u.Aspnetuserid = user.Id;
            u.Firstname = patientInfoModel.firstName;
            u.Lastname = patientInfoModel.lastName;
            u.Email = patientInfoModel.email;
            u.Mobile = patientInfoModel.phoneNo;
            u.Street = patientInfoModel.street;
            u.City = patientInfoModel.city;
            u.State = patientInfoModel.state;
            u.Zipcode = patientInfoModel.zipCode;
            u.Createdby = user.Username;
            u.Createddate = DateTime.Now;
            //u.roomno = patientInfoModel.roomno;

            _db.Users.Add(u);
            _db.SaveChanges();
        }
    }
}
