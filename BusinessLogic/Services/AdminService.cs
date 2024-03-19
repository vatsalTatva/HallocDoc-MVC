﻿using BusinessLogic.Interfaces;
using DataAccess.CustomModels;
using DataAccess.Data;
using DataAccess.Enums;
using DataAccess.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BusinessLogic.Services
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _db;

        public AdminService(ApplicationDbContext db)
        {
            _db = db;
        }

        public Aspnetuser GetAspnetuser(string email)
        {
            var aspNetUser = _db.Aspnetusers.Include(x => x.Aspnetuserroles).FirstOrDefault(x => x.Email == email);
            return aspNetUser;
        }

        public List<AdminDashTableModel> GetRequestsByStatus(int tabNo)
        {
            var query = from r in _db.Requests
                        join rc in _db.Requestclients on r.Requestid equals rc.Requestid
                        //where r.Status == status
                        select new AdminDashTableModel
                        {
                            firstName = rc.Firstname,
                            lastName = rc.Lastname,
                            intDate = rc.Intdate,
                            intYear = rc.Intyear,
                            strMonth = rc.Strmonth,
                            requestorFname = r.Firstname,
                            requestorLname = r.Lastname,
                            createdDate = r.Createddate,
                            mobileNo = rc.Phonenumber,
                            city = rc.City,
                            state = rc.State,
                            street = rc.Street,
                            zipCode = rc.Zipcode,
                            requestTypeId = r.Requesttypeid,
                            status = r.Status,
                            requestClientId = rc.Requestclientid,
                            reqId = r.Requestid
                        };


            if (tabNo == 1)
            {
                 
                query = query.Where(x => x.status == (int)StatusEnum.Unassigned);
            }
              
            else if (tabNo == 2)
            {
               
                query = query.Where(x => x.status == (int)StatusEnum.Accepted);
            }
            else if (tabNo == 3)
            {
              
                query = query.Where(x => x.status == (int)StatusEnum.MDEnRoute || x.status == (int)StatusEnum.MDOnSite);
            }
            else if (tabNo == 4)
            {

                query = query.Where(x => x.status == (int)StatusEnum.Conclude);
            }
            else if (tabNo == 5)
            {

                query = query.Where(x => (x.status == (int)StatusEnum.Cancelled || x.status == (int)StatusEnum.CancelledByPatient) || x.status == (int)StatusEnum.Closed);
            }
            else if (tabNo == 6)
            {

                query = query.Where(x => x.status == (int)StatusEnum.Unpaid);
            }



            var result = query.ToList();


            return result;
        }

        public StatusCountModel GetStatusCount()
        {
            var requestsWithClients = _db.Requests
     .Join(_db.Requestclients,
         r => r.Requestid,
         rc => rc.Requestid,
         (r, rc) => new { Request = r, RequestClient = rc })
     .ToList();

            StatusCountModel statusCount = new StatusCountModel
            {
                NewCount = requestsWithClients.Count(x => x.Request.Status == (int)StatusEnum.Unassigned),
                PendingCount = requestsWithClients.Count(x => x.Request.Status == (int)StatusEnum.Accepted),
                ActiveCount = requestsWithClients.Count(x => x.Request.Status == (int)StatusEnum.MDEnRoute || x.Request.Status == (int)StatusEnum.MDOnSite),
                ConcludeCount = requestsWithClients.Count(x => x.Request.Status == (int)StatusEnum.Conclude),
                ToCloseCount = requestsWithClients.Count(x => (x.Request.Status == (int)StatusEnum.Cancelled || x.Request.Status == (int)StatusEnum.CancelledByPatient) || x.Request.Status == (int)StatusEnum.Closed),
                UnpaidCount = requestsWithClients.Count(x => x.Request.Status == (int)StatusEnum.Unpaid)
            };

            return statusCount;


        }

        public bool UpdateAdminNotes(string additionalNotes, int reqId)
        {
            var reqNotes = _db.Requestnotes.FirstOrDefault(x => x.Requestid == reqId);
            try
            {

                if (reqNotes == null)
                {
                    Requestnote rn = new Requestnote();
                    rn.Requestid = reqId;
                    rn.Adminnotes = additionalNotes;
                    rn.Createdby = "admin";
                    //here instead of admin , add id of the admin through which admin is loggedIn 
                    rn.Createddate = DateTime.Now;
                    _db.Requestnotes.Add(rn);
                    _db.SaveChanges();
                }
                else
                {
                    reqNotes.Adminnotes = additionalNotes;
                    reqNotes.Modifieddate = DateTime.Now;
                    reqNotes.Modifiedby = "admin";
                    //here instead of admin , add id of the admin through which admin is loggedIn 
                    _db.Requestnotes.Update(reqNotes);
                    _db.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public ViewCaseViewModel ViewCaseViewModel(int Requestclientid ,int RequestTypeId)
        {
            Requestclient obj = _db.Requestclients.FirstOrDefault(x => x.Requestclientid == Requestclientid);
            ViewCaseViewModel viewCaseViewModel = new()
            {
                Requestid=obj.Requestid,
                Requestclientid = obj.Requestclientid,
                Firstname = obj.Firstname,
                Lastname = obj.Lastname,
                Email = obj.Email,
                Phonenumber = obj.Phonenumber,
                City = obj.City,
                Street = obj.Street,
                State = obj.State,
                Zipcode = obj.Zipcode,
                Room = obj.Address,
                Notes = obj.Notes,
                RequestTypeId = RequestTypeId
            };
            return viewCaseViewModel;
        }

        public ViewNotesModel ViewNotes(int ReqId)
        {
          
            var requestNotes = _db.Requestnotes.Where(x => x.Requestid == ReqId).FirstOrDefault();
            var statuslogs = _db.Requeststatuslogs.Where(x => x.Requestid == ReqId).ToList();
            ViewNotesModel model =new ViewNotesModel();
            if (model == null)
            {
                model.TransferNotesList = null;
                model.PhysicianNotes = null;
                model.AdminNotes = null;               
            }

            
            if (requestNotes != null)
            {
                model.PhysicianNotes = requestNotes.Physiciannotes;
                model.AdminNotes = requestNotes.Adminnotes;
            }
            if (statuslogs != null)
            {
                model.TransferNotesList = statuslogs;
            }
        
            return model;
        }
        public CancelCaseModel CancelCase(int reqId)
        {
            var casetags = _db.Casetags.ToList();
            var request = _db.Requests.Where(x => x.Requestid == reqId).FirstOrDefault();
            CancelCaseModel model = new()
            {
                PatientFName = request.Firstname,
                PatientLName = request.Lastname,
                casetaglist = casetags

            };
            return model;
        }

        public bool SubmitCancelCase(CancelCaseModel cancelCaseModel)
        {
            try
            {
                var req = _db.Requests.Where(x => x.Requestid == cancelCaseModel.reqId).FirstOrDefault();
                req.Status = (int)StatusEnum.Cancelled;
                req.Casetag = cancelCaseModel.casetag.ToString();
                req.Modifieddate = DateTime.Now;
                //var reqStatusLog = _db.Requeststatuslogs.Where(x => x.Requestid == cancelCaseModel.reqId).FirstOrDefault();
                //if (reqStatusLog == null) {
                Requeststatuslog rsl = new Requeststatuslog();
                rsl.Requestid = (int)cancelCaseModel.reqId;
                rsl.Status = (int)StatusEnum.Cancelled;
                rsl.Notes = cancelCaseModel.notes;
                rsl.Createddate = DateTime.Now;
                _db.Requeststatuslogs.Add(rsl);
                _db.Requests.Update(req);
                _db.SaveChanges();
                return true;
                //}
                //else
                //{
                //    reqStatusLog.Status = (int)StatusEnum.Cancelled;
                //    reqStatusLog.Notes = cancelCaseModel.notes;
                   
                //    _db.Requeststatuslogs.Update(reqStatusLog);
                //    _db.Requests.Update(req);
                //    _db.SaveChanges();
                //    return true;
                //}

                
               
               
            }
            catch (Exception ex)
            {
                return false;
            }
        
        }

        public AssignCaseModel AssignCase(int reqId)
        {

            var regionlist = _db.Regions.ToList();
            AssignCaseModel assignCaseModel = new()
            { 
                regionList = regionlist,

            };
            return assignCaseModel;
        }

        public List<Physician> GetPhysicianByRegion(int Regionid)
        {
            
            var physicianList = _db.Physicianregions.Where(x => x.Regionid == Regionid).Select(x => x.Physician).ToList();
           
            return physicianList;


        }

        public bool SubmitAssignCase(AssignCaseModel assignCaseModel)
        {
            try
            {

                var req  = _db.Requests.Where(x => x.Requestid == assignCaseModel.ReqId).FirstOrDefault();
                req.Status = (int)StatusEnum.Accepted;
                req.Physicianid = assignCaseModel.selectPhysicianId;
                req.Modifieddate = DateTime.Now;
                
                //var reqStatusLog = _db.Requeststatuslogs.Where(x => x.Requestid == assignCaseModel.ReqId).FirstOrDefault();
                //if (reqStatusLog == null)
                //{
                Requeststatuslog rsl = new Requeststatuslog();
                rsl.Requestid = (int)assignCaseModel.ReqId;
                rsl.Status = (int)StatusEnum.Accepted;
                rsl.Notes = assignCaseModel.description;
                rsl.Physicianid = assignCaseModel.selectPhysicianId;
                rsl.Createddate = DateTime.Now;
                _db.Requeststatuslogs.Add(rsl);
                _db.Requests.Update(req);
                _db.SaveChanges();
                return true;
                //}
                //else
                //{
                //    reqStatusLog.Status = (int)StatusEnum.Accepted;
                //    reqStatusLog.Notes = assignCaseModel.description;
                //    reqStatusLog.Physicianid = assignCaseModel.selectPhysicianId;

                //    _db.Requeststatuslogs.Update(reqStatusLog);
                //    _db.Requests.Update(req);
                //    _db.SaveChanges();
                //    return true;
                //}

               
            }
            catch(Exception ex) {
                return false;
            }

        }

        public BlockCaseModel BlockCase(int reqId)
        {
            var reqClient = _db.Requestclients.Where(x => x.Requestid==reqId).FirstOrDefault();
            BlockCaseModel model = new()
            {
                ReqId = reqId,
                firstName = reqClient.Firstname,
                lastName= reqClient.Lastname,
                reason=null
            };

            return model;
        }

        public bool SubmitBlockCase(BlockCaseModel blockCaseModel)
        {
            try
            {
                var request = _db.Requests.FirstOrDefault(r => r.Requestid == blockCaseModel.ReqId);
                if(request != null)
                {
                    if (request.Isdeleted == null)
                    {
                        request.Isdeleted = new BitArray(1);
                        request.Isdeleted[0] = true;
                        request.Status = (int)StatusEnum.Clear;
                        request.Modifieddate = DateTime.Now;

                        _db.Requests.Update(request);
                       
                    }
                    Blockrequest blockrequest = new Blockrequest();

                    blockrequest.Phonenumber = request.Phonenumber==null?"+91":request.Phonenumber;
                    blockrequest.Email = request.Email;
                    blockrequest.Reason = blockCaseModel.reason;
                    blockrequest.Requestid = (int)blockCaseModel.ReqId;
                    blockrequest.Createddate = DateTime.Now;
                   
                    _db.Blockrequests.Add(blockrequest);
                    _db.SaveChanges();
                    return true;
                   
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public ViewUploadModel GetAllDocById(int requestId)
        {
            
            var list = _db.Requestwisefiles.Where(x => x.Requestid == requestId).ToList();  
            var reqClient = _db.Requestclients.Where(x => x.Requestid == requestId).FirstOrDefault();

            ViewUploadModel result = new()
            {
                files = list,
                firstName = reqClient.Firstname,
                lastName = reqClient.Lastname,

            };

            return result;
            
        }

        public bool UploadFiles(List<IFormFile> files ,int reqId)
        {

            try
            {
                if (files != null)
                {
                    foreach (IFormFile file in files)
                    {
                        if (file != null && file.Length > 0)
                        {
                            //get file name
                            var fileName = Path.GetFileName(file.FileName);

                            //define path
                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedFiles", fileName);

                            // Copy the file to the desired location
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                file.CopyTo(stream)
                       ;
                            }

                            Requestwisefile requestwisefile = new()
                            {
                                Filename = fileName,
                                Requestid = reqId,
                                Createddate = DateTime.Now
                            };

                            _db.Requestwisefiles.Add(requestwisefile);
                            
                        }
                    }
                    _db.SaveChanges();
                    return true;
                }
                else {return false; }

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteFileById(int reqFileId)
        {
            try
            {
                var reqWiseFile = _db.Requestwisefiles.Where(x=>x.Requestwisefileid == reqFileId).FirstOrDefault();
                if (reqWiseFile != null)
                {
                    _db.Requestwisefiles.Remove(reqWiseFile);
                    _db.SaveChanges();
                   return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteAllFiles(List<string> filenames, int reqId)
        {
            try
            {
                var list = _db.Requestwisefiles.Where(x => x.Requestid == reqId).ToList();

                foreach(var filename in filenames)
                {
                    var existFile = list.Where(x => x.Filename == filename && x.Requestid == reqId).FirstOrDefault();
                    if (existFile != null)
                    {
                        list.Remove(existFile);
                        _db.Requestwisefiles.Remove(existFile);
                    }
                }
                _db.SaveChanges();
                return true;               
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Order FetchProfession()
        {
            
           
            var Healthprofessionaltype = _db.Healthprofessionaltypes.ToList();

            Order order = new()
            {
                Profession = Healthprofessionaltype
               
            };
            return order;
        }
        public JsonArray FetchVendors(int proffesionId)
        {
            var result = new JsonArray();
            IEnumerable<Healthprofessional> businesses = _db.Healthprofessionals.Where(prof => prof.Profession == proffesionId);

            foreach (Healthprofessional business in businesses)
            {
                result.Add(new { businessId = business.Vendorid, businessName = business.Vendorname });
            }
            return result;
        }

        public Healthprofessional VendorDetails(int selectedValue)
        {
            Healthprofessional business = _db.Healthprofessionals.First(prof => prof.Vendorid == selectedValue);

            return business;
        }

        public bool SendOrder(Order order)
        {
            try
            {
                Orderdetail od = new Orderdetail();
                od.Vendorid = order.BusinessId;
                od.Requestid = order.ReqId;
                od.Faxnumber = order.faxnumber;
                od.Email = order.email;
                od.Businesscontact = order.BusineesContact;
                od.Prescription = order.orderDetail;
                od.Noofrefill = order.RefilNo;
                od.Createddate = DateTime.Now;
                od.Createdby = "Admin";

                _db.Orderdetails.Add(od);
                _db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool ClearCase(int reqId)
        {
            try
            {
                var request = _db.Requests.FirstOrDefault(x=>x.Requestid==reqId);
                if (request != null)
                {
                    request.Status = (int)StatusEnum.Clear;
                    _db.Requests.Update(request);
                    _db.SaveChanges();
                    return true;
                }
                return false;
            }catch(Exception ex)
            {
                return false;
            }
        }

        public SendAgreementModel SendAgreementCase(int reqId)
        {
            var requestClient = _db.Requestclients.Where(x => x.Requestid == reqId).FirstOrDefault();
            SendAgreementModel obj = new();
            obj.Reqid = reqId;
            obj.PhoneNumber = requestClient.Phonenumber;
            obj.Email = requestClient.Email;

            return obj;
        }

        public CloseCaseModel ShowCloseCase(int reqId)
        {
            var rc = _db.Requestclients.FirstOrDefault(x=>x.Requestid == reqId);
            var list = _db.Requestwisefiles.Where(x => x.Requestid == reqId).ToList();
            string date = null;
            if ((rc.Intyear != null && rc.Intdate != null) && rc.Strmonth != null)
            {
                date = new DateTime((int)(rc.Intyear), Convert.ToInt16(rc.Strmonth), (int)(rc.Intdate)).ToString("yyyy-MM-dd");
            }
            CloseCaseModel model = new()
            {
                reqid= reqId,
                fname= rc.Firstname,
                lname= rc.Lastname,
                email= rc.Email,
                phoneNo= rc.Phonenumber,
                files=list,
               
                fulldateofbirth = date,



            };

            return model;
        }

        public bool SaveCloseCase(CloseCaseModel model)
        {
            try
            {
                var reqClient = _db.Requestclients.FirstOrDefault(x => x.Requestid == model.reqid);
                reqClient.Phonenumber = model.phoneNo;
                reqClient.Email=model.email;
                _db.Requestclients.Update(reqClient);
                _db.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public bool SubmitCloseCase(int ReqId)
        {
            try
            {
                var request = _db.Requests.FirstOrDefault(x=>x.Requestid== ReqId);
                request.Status = (int)StatusEnum.Unpaid;
                _db.Requests.Update(request);
                _db.Requeststatuslogs.Add(new Requeststatuslog()
                {
                    Requestid=ReqId,
                    Status = (int)StatusEnum.Unpaid,
                    Notes="Case closed and unpaid",
                    Createddate = DateTime.Now,
                });
                _db.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public int GetStatusForReviewAgreement(int reqId)
        {
            var status = (int)_db.Requests.Where(x => x.Requestid == reqId).Select(x => x.Status).FirstOrDefault();
            return status;
        }

        public bool AgreeAgreement(AgreementModel model)
        {
            try
            {
                var req = _db.Requests.FirstOrDefault(x => x.Requestid == model.reqId);
               

                req.Status = (int)StatusEnum.MDEnRoute;
                req.Modifieddate= DateTime.Now;
                Requeststatuslog rsl = new Requeststatuslog();
                rsl.Requestid = req.Requestid;
                rsl.Status = (int)StatusEnum.MDEnRoute;
                rsl.Createddate = DateTime.Now;
                rsl.Notes = "Agreement accepted by patient";
                _db.Requests.Update(req);
                _db.Requeststatuslogs.Add(rsl);
                _db.SaveChanges();
                return true;
            }

            catch (Exception e)
            {
                return false;
            }

        }


        public AgreementModel CancelAgreement(int reqId)
        {
            var requestclient = _db.Requestclients.FirstOrDefault(x => x.Requestid == reqId);
            AgreementModel model = new()
            {
                reqId = reqId,
                fName = requestclient.Firstname,
                lName = requestclient.Lastname,
            };
            return model;
        }

        public bool SubmitCancelAgreement(AgreementModel model)
        {
            try
            {
                var request = _db.Requests.FirstOrDefault(x => x.Requestid == model.reqId);


                if (request != null)
                {
                    request.Status = (int)StatusEnum.CancelledByPatient;
                    request.Modifieddate = DateTime.Now;
                    Requeststatuslog rsl = new Requeststatuslog();
                    rsl.Requestid = request.Requestid;
                    rsl.Status = (int)StatusEnum.CancelledByPatient;
                    rsl.Notes = model.reason;
                    rsl.Createddate = DateTime.Now;

                    _db.Requests.Update(request);
                    _db.Requeststatuslogs.Add(rsl);
                    _db.SaveChanges();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public EncounterFormModel EncounterForm(int reqId)
        {
            var reqClient = _db.Requestclients.FirstOrDefault(x => x.Requestid == reqId);
            var encForm = _db.Encounterforms.FirstOrDefault(x => x.Requestid == reqId);
            EncounterFormModel ef = new EncounterFormModel();
            ef.reqid = reqId;
            ef.FirstName = reqClient.Firstname;
            ef.LastName = reqClient.Lastname;
            ef.Location = reqClient.Street + reqClient.City + reqClient.State + reqClient.Zipcode;
            //ef.BirthDate = new DateTime((int)(reqClient.Intyear), Convert.ToInt16(reqClient.Strmonth), (int)(reqClient.Intdate)).ToString("yyyy-MM-dd");
            ef.PhoneNumber = reqClient.Phonenumber;
            ef.Email = reqClient.Email;
            if(encForm!= null )
            {
                ef.HistoryIllness = encForm.Illnesshistory;
                ef.MedicalHistory = encForm.Medicalhistory;
                //ef.Date = new DateTime((int)(encForm.Intyear), Convert.ToInt16(encForm.Strmonth), (int)(encForm.Intdate)).ToString("yyyy-MM-dd");
                ef.Medications = encForm.Medications;
                ef.Allergies = encForm.Allergies;
                ef.Temp = encForm.Temperature;
                ef.Hr = encForm.Heartrate;
                ef.Rr = encForm.Respirationrate;
                ef.BpS = encForm.Bloodpressuresystolic;
                ef.BpD = encForm.Bloodpressurediastolic;
                ef.O2 = encForm.Oxygenlevel;
                ef.Pain = encForm.Pain;
                ef.Heent = encForm.Heent;
                ef.Cv = encForm.Cardiovascular;
                ef.Chest = encForm.Chest;
                ef.Abd = encForm.Abdomen;
                ef.Extr = encForm.Extremities;
                ef.Skin = encForm.Skin;
                ef.Neuro = encForm.Neuro;
                ef.Other = encForm.Other;
                ef.Diagnosis = encForm.Diagnosis;
                ef.TreatmentPlan = encForm.Treatmentplan;
                ef.MedicationDispensed = encForm.Medicationsdispensed;
                ef.Procedures = encForm.Procedures;
                ef.FollowUp = encForm.Followup;
            }
            return ef;
        }

        public bool SubmitEncounterForm(EncounterFormModel model)
        {
            try
            {
                //concludeEncounter _obj = new concludeEncounter();

                var ef = _db.Encounterforms.FirstOrDefault(r => r.Requestid == model.reqid);


                if (ef == null)
                {
                    Encounterform _encounter = new Encounterform()
                    {
                        Requestid = model.reqid,
                        Firstname = model.FirstName,
                        Lastname = model.LastName,
                        Location = model.Location,
                        Phonenumber = model.PhoneNumber,
                        Email = model.Email,
                        Illnesshistory = model.HistoryIllness,
                        Medicalhistory = model.MedicalHistory,
                        //date = model.Date,
                        Medications = model.Medications,
                        Allergies = model.Allergies,
                        Temperature = model.Temp,
                        Heartrate = model.Hr,
                        Respirationrate = model.Rr,
                        Bloodpressuresystolic = model.BpS,
                        Bloodpressurediastolic = model.BpD,
                        Oxygenlevel = model.O2,
                        Pain = model.Pain,
                        Heent = model.Heent,
                        Cardiovascular = model.Cv,
                        Chest = model.Chest,
                        Abdomen = model.Abd,
                        Extremities = model.Extr,
                        Skin = model.Skin,
                        Neuro = model.Neuro,
                        Other = model.Other,
                        Diagnosis = model.Diagnosis,
                        Treatmentplan = model.TreatmentPlan,
                        Medicationsdispensed = model.MedicationDispensed,
                        Procedures = model.Procedures,
                        Followup = model.FollowUp,
                        Isfinalized = false
                    };

                    _db.Encounterforms.Add(_encounter);

                    //_obj.indicate = true;
                }
                else
                {
                    
                    var efdetail = _db.Encounterforms.FirstOrDefault(x=>x.Requestid==model.reqid);
                    
                    efdetail.Requestid = model.reqid;
                    efdetail.Illnesshistory = model.HistoryIllness;
                    efdetail.Medicalhistory = model.MedicalHistory;
                    //efdetail.Date = model.Date;
                    efdetail.Medications = model.Medications;
                    efdetail.Allergies = model.Allergies;
                    efdetail.Temperature = model.Temp;
                    efdetail.Heartrate = model.Hr;
                    efdetail.Respirationrate = model.Rr;
                    efdetail.Bloodpressuresystolic = model.BpS;
                    efdetail.Bloodpressurediastolic = model.BpD;
                    efdetail.Oxygenlevel = model.O2;
                    efdetail.Pain = model.Pain;
                    efdetail.Heent = model.Heent;
                    efdetail.Cardiovascular = model.Cv;
                    efdetail.Chest = model.Chest;
                    efdetail.Abdomen = model.Abd;
                    efdetail.Extremities = model.Extr;
                    efdetail.Skin = model.Skin;
                    efdetail.Neuro = model.Neuro;
                    efdetail.Other = model.Other;
                    efdetail.Diagnosis = model.Diagnosis;
                    efdetail.Treatmentplan = model.TreatmentPlan;
                    efdetail.Medicationsdispensed = model.MedicationDispensed;
                    efdetail.Procedures = model.Procedures;
                    efdetail.Followup = model.FollowUp;
                    efdetail.Modifieddate = DateTime.Now;
                    ef.Isfinalized = false;
                    _db.Encounterforms.Update(efdetail);
                    // _obj.indicate = true;
                };


                _db.SaveChanges();

                return true;
            }
            catch (Exception ex) {
                return false;
            }

        }

        public MyProfileModel MyProfile(string sessionEmail)
        {
            var myProfileMain = _db.Admins.Where(x => x.Email == sessionEmail).Select(x => new MyProfileModel()
            {
                fname = x.Firstname,
                lname = x.Lastname,
                email = x.Email,
                confirm_email = x.Email,
                mobile_no = x.Mobile,
                addr1 = x.Address1,
                addr2 = x.Address2,
                city=x.City,
                zip = x.Zip,
                state = _db.Regions.Where(r => r.Regionid == x.Regionid).Select(r => r.Name).First(),
                roles = _db.Aspnetroles.ToList(),
            }).ToList().FirstOrDefault();

            var aspnetuser = _db.Aspnetusers.Where(r => r.Email == sessionEmail).First();

           

            myProfileMain.username = aspnetuser.Username;
            myProfileMain.password = aspnetuser.Passwordhash;

            return myProfileMain;
        }


    }

}
