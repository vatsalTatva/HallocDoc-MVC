using AspNetCoreHero.ToastNotification.Abstractions;
using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using DataAccess.CustomModels;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using HalloDoc.mvc.Auth;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json.Nodes;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HalloDoc.mvc.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly INotyfService _notyf;
        private readonly IAdminService _adminService;
        private readonly IPatientService _patientService;
        private readonly IJwtService _jwtService;


        public AdminController(ILogger<AdminController> logger ,INotyfService notyfService , IAdminService adminService , IPatientService patientService,IJwtService jwtService)
        {
            _logger = logger;
            _notyf = notyfService;
            _adminService = adminService;
            _patientService = patientService;
            _jwtService = jwtService;
        }

        public IActionResult Index()
        {
            return View();
        }
        public static string GenerateSHA256(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            using (var hashEngine = SHA256.Create())
            {
                var hashedBytes = hashEngine.ComputeHash(bytes, 0, bytes.Length);
                var sb = new StringBuilder();
                foreach (var b in hashedBytes)
                {
                    var hex = b.ToString("x2");
                    sb.Append(hex);
                }
                return sb.ToString();
            }
        }

        [HttpGet]
        public IActionResult AdminLogin()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AdminLogin(AdminLoginModel adminLoginModel)
        {
            if (ModelState.IsValid)
            {
                var aspnetuser = _adminService.GetAspnetuser(adminLoginModel.email);
                if (aspnetuser != null)
                {
                    adminLoginModel.password = GenerateSHA256(adminLoginModel.password);
                    if (aspnetuser.Passwordhash == adminLoginModel.password)
                    {
                        var jwtToken = _jwtService.GetJwtToken(aspnetuser);
                        Response.Cookies.Append("jwt", jwtToken);
                        _notyf.Success("Logged in Successfully");
                        return RedirectToAction("AdminDashboard", "Admin");
                    }
                    else
                    {
                        _notyf.Error("Password is incorrect");

                        return View();
                    }
                }
                _notyf.Error("Email is incorrect");
                return View();
            }
            else
            {
                return View(adminLoginModel);
            }
        }

        [CustomAuthorize("Admin")]
        public IActionResult AdminDashboard()
        {
           
            return View();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToAction("AdminLogin", "Admin");
        }

        public IActionResult GetCount()
        {
            var statusCountModel = _adminService.GetStatusCount();
            return PartialView("_AllRequests", statusCountModel);
        }
        public IActionResult GetRequestsByStatus(int tabNo, int CurrentPage)
        {
            var list = _adminService.GetRequestsByStatus(tabNo, CurrentPage);
            
            if (tabNo == 0)
            {
                return Json(list);
            }
            if (tabNo == 1)
            {
                return PartialView("_NewRequest", list);
            }
            else if (tabNo == 2)
            {
                return PartialView("_PendingRequest", list);
            }
            else if (tabNo == 3)
            {
                return PartialView("_ActiveRequest", list);
            }
            else if (tabNo == 4)
            {
                return PartialView("_ConcludeRequest", list);
            }
            else if (tabNo == 5)
            {
                return PartialView("_ToCloseRequest", list);
            }
            else if (tabNo == 6)
            {
                return PartialView("_UnpaidRequest", list);
            }
            return View();
        }

        public IActionResult FilterRegion(int regionId,int tabNo)
        {
            var list = _adminService.GetRequestByRegion(regionId, tabNo);
            return PartialView("_NewRequest", list);
        }

        [HttpPost]
        public string ExportReq(List<AdminDashTableModel> reqList)
        {
            StringBuilder stringbuild = new StringBuilder();

            string header = "\"No\"," + "\"Name\"," + "\"DateOfBirth\"," + "\"Requestor\"," +
                "\"RequestDate\"," + "\"Phone\"," + "\"Notes\"," + "\"Address\","
                 + "\"Physician\"," + "\"DateOfService\"," + "\"Region\"," +
                "\"Status\"," + "\"RequestTypeId\"," + "\"OtherPhone\"," + "\"Email\"," + "\"RequestId\"," + Environment.NewLine + Environment.NewLine;

            stringbuild.Append(header);
            int count = 1;

            foreach (var item in reqList)
            {
                string content = $"\"{count}\"," + $"\"{item.firstName}\"," + $"\"{item.intDate}\"," + $"\"{item.requestorFname}\"," +
                    $"\"{item.intDate}\"," + $"\"{item.mobileNo}\"," + $"\"{item.notes}\"," + $"\"{item.street}\"," +
                    $"\"{item.lastName}\"," + $"\"{item.intDate}\"," + $"\"{item.street}\"," +
                    $"\"{item.status}\"," + $"\"{item.requestTypeId}\"," + $"\"{item.mobileNo}\"," + $"\"{item.firstName}\"," + $"\"{item.reqId}\"," + Environment.NewLine;

                count++;
                stringbuild.Append(content);
            }

            string finaldata = stringbuild.ToString();

            return finaldata;
           
        }

        [HttpGet]
        public IActionResult SendLink()
        {
            return PartialView("_SendLink");
        }
        [HttpPost]
        public IActionResult SendLink(SendLinkModel model)
        {
            bool isSend=false;
            try
            {
                string baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
                string reviewPathLink = baseUrl + Url.Action("RequestScreen", "Patient");

                SendEmail(model.email, "Create Patient Request", $"Hello, please create patient request from this link: {reviewPathLink}");
                _notyf.Success("Link Sent");
                isSend = true;
            }
            catch (Exception ex)
            {
                _notyf.Error("Failed to sent");
            }
            return Json(new {isSend=isSend});
           
        }
        public IActionResult ViewCase(int Requestclientid,int RequestTypeId)
        {
            var model = _adminService.ViewCaseViewModel(Requestclientid,RequestTypeId);

            return View(model);
        }

        [HttpPost]
        public IActionResult UpdateNotes(ViewNotesModel model)
        {
            int? reqId = HttpContext.Session.GetInt32("RNId");
            bool isUpdated = _adminService.UpdateAdminNotes(model.AdditionalNotes, (int)reqId);
            if (isUpdated)
            {
                _notyf.Success("Saved Changes !!");
                return RedirectToAction("ViewNote", "Admin", new { ReqId = reqId });

            }
            return View();
        }

        public IActionResult ViewNote(int ReqId)
        {
            HttpContext.Session.SetInt32("RNId", ReqId);
            ViewNotesModel data = _adminService.ViewNotes(ReqId);
            return View(data);
        }

        public IActionResult CancelCase(int reqId)
        {
           
            var model = _adminService.CancelCase(reqId);
            model.reqId = reqId;
            return PartialView("_CancelCase", model);
        }

        
        [HttpPost]
        public IActionResult SubmitCancelCase(CancelCaseModel cancelCaseModel,int reqId)
        {
            cancelCaseModel.reqId = reqId;
            bool isCancelled = _adminService.SubmitCancelCase(cancelCaseModel);

            if (isCancelled)
            {
                _notyf.Success("Cancelled successfully");
                return RedirectToAction("AdminDashboard");
            }
            _notyf.Error("Cancellation Failed");
            return RedirectToAction("AdminDashboard");
        }

        [HttpGet]
        public IActionResult AssignCase(int reqId)
        {
            HttpContext.Session.SetInt32("AssignReqId", reqId);
            var model  = _adminService.AssignCase(reqId);
            return PartialView("_AssignCase",model);
        }

        public IActionResult GetPhysician(int selectRegion)
        {
            List<Physician> physicianlist = _adminService.GetPhysicianByRegion(selectRegion);
            return Json(new {physicianlist});
        }

        [HttpPost]
        public IActionResult SubmitAssignCase(AssignCaseModel assignCaseModel)
        {
            assignCaseModel.ReqId = HttpContext.Session.GetInt32("AssignReqId");
            bool isAssigned = _adminService.SubmitAssignCase(assignCaseModel);
            if(isAssigned)
            {
                _notyf.Success("Assigned successfully");
                return RedirectToAction("AdminDashboard", "Admin");
            }
            return View();
        }

        public IActionResult BlockCase(int reqId)
        {
           
            var model = _adminService.BlockCase(reqId);
            return PartialView("_BlockCase",model);
        }

        [HttpPost]
        public IActionResult SubmitBlockCase(BlockCaseModel blockCaseModel,int reqId)
        {
            blockCaseModel.ReqId = reqId;
            bool isBlocked = _adminService.SubmitBlockCase(blockCaseModel);
            if (isBlocked)
            {
                _notyf.Success("Blocked Successfully");
                return RedirectToAction("AdminDashboard", "Admin");
            }
            _notyf.Error("BlockCase Failed");
            return RedirectToAction("AdminDashboard", "Admin");
        }


        public IActionResult ViewUploads(int reqId)
        {
            HttpContext.Session.SetInt32("rid", reqId);
            var model = _adminService.GetAllDocById(reqId);
            return View(model);
        }

        [HttpPost]
        public IActionResult UploadFiles(ViewUploadModel model)
        {
            var rid = (int)HttpContext.Session.GetInt32("rid");
            if(model.uploadedFiles== null)
            {
                _notyf.Error("First Upload Files");
                return RedirectToAction("ViewUploads", "Admin", new { reqId = rid });
            }
            bool isUploaded =  _adminService.UploadFiles(model.uploadedFiles, rid);
            if (isUploaded)
            {
                _notyf.Success("Uploaded Successfully");
                return RedirectToAction("ViewUploads", "Admin", new { reqId = rid });
            }
            else
            {
                _notyf.Error("Upload Failed");
                return RedirectToAction("ViewUploads", "Admin", new { reqId = rid });
            }
        }

        public IActionResult DeleteFileById(int id)
        {
            var rid = (int)HttpContext.Session.GetInt32("rid");
            bool isDeleted = _adminService.DeleteFileById(id);
            if (isDeleted)
            {
                return RedirectToAction("ViewUploads", "Admin", new { reqId = rid });
            }
            else
            {
                _notyf.Error("SomeThing Went Wrong");
                return RedirectToAction("ViewUploads", "Admin", new { reqId = rid });
            }
        }

        public IActionResult DeleteAllFiles(List<string> selectedFiles)
        {
            var rid = (int)HttpContext.Session.GetInt32("rid");
            bool isDeleted = _adminService.DeleteAllFiles(selectedFiles, rid);
            if (isDeleted)
            {
                _notyf.Success("Deleted Successfully");
                return RedirectToAction("ViewUploads", "Admin", new { reqId = rid });
            }
            _notyf.Error("SomeThing Went Wrong");
            return RedirectToAction("ViewUploads", "Admin", new { reqId = rid });

        }

        public IActionResult SendAllFiles(List<string> selectedFiles) 
        {
            var rid = (int)HttpContext.Session.GetInt32("rid");

           
            //var message = string.Join(", ", selectedFiles);
            SendEmail("yashvariya23@gmail.com", "Documents", selectedFiles);
            _notyf.Success("Send Mail Successfully");
            return RedirectToAction("ViewUploads", "Admin", new { reqId = rid });
        }

        public Task SendEmail(string email, string subject, List<string> filenames)
        {
            var mail = "tatva.dotnet.vatsalgadoya@outlook.com";
            var password = "VatsalTatva@2024";

            var client = new SmtpClient("smtp.office365.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, password)
            };

            MailMessage mailMessage = new MailMessage();
            for (var i = 0; i < filenames.Count; i++)
            {
                string pathname = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedFiles", filenames[i]);
                Attachment attachment = new Attachment(pathname);
                mailMessage.Attachments.Add(attachment);
            }
            mailMessage.To.Add(email);
            mailMessage.From = new MailAddress(mail);

            mailMessage.Subject=subject;


            return client.SendMailAsync(mailMessage);
        }

        [HttpGet]
        public IActionResult Order(int reqId)
        {
            var order = _adminService.FetchProfession();
            order.ReqId =reqId;
            return View(order);
        }

        [HttpGet]
        public JsonArray FetchBusiness(int proffesionId)
        {
            var result = _adminService.FetchVendors(proffesionId);
            return result;
        }

        [HttpGet]
        public Healthprofessional VendorDetails(int selectedValue)
        {
            var result = _adminService.VendorDetails(selectedValue);
            return result;
        }

        [HttpPost]
        public IActionResult Order(Order order)
        {
            bool isSend = _adminService.SendOrder(order);
            return Json(new { isSend = isSend });
        }

        [HttpGet]
        public IActionResult TransferCase(int reqId)
        {
            var model = _adminService.AssignCase(reqId);
            model.ReqId = reqId;
            return PartialView("_TransferCase", model);
        }

        [HttpPost]
        public IActionResult SubmitTransferCase(AssignCaseModel transferCaseModel)
        {            
            bool isTransferred = _adminService.SubmitAssignCase(transferCaseModel);
            return Json(new { isTransferred = isTransferred });
        }


        [HttpGet]
        public IActionResult ClearCase(int reqId)
        {
            ViewBag.ClearCaseId = reqId;
            return PartialView("_ClearCase");
        }

        [HttpPost]
        public IActionResult SubmitClearCase(int reqId)
        {
            bool isClear =  _adminService.ClearCase(reqId);
            if(isClear)
            {
                _notyf.Success("Cleared SUccessfully");
                return RedirectToAction("AdminDashboard");
            }
            _notyf.Error("Failed");
            return RedirectToAction("AdminDashboard");
        }

        [HttpGet]
        public IActionResult SendAgreement(int reqId,int reqType)
        {
            var model = _adminService.SendAgreementCase(reqId);
            model.reqType = reqType;
            return PartialView("_sendagreement", model);
        }

        public Task SendEmail(string email, string subject, string message)
        {
            var mail = "tatva.dotnet.vatsalgadoya@outlook.com";
            var password = "VatsalTatva@2024";

            var client = new SmtpClient("smtp.office365.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, password)
            };



            return client.SendMailAsync(new MailMessage(from: mail, to: email, subject, message));
        }


        [HttpPost]
        public IActionResult SendAgreement(SendAgreementModel model)
        {
            try
            {
                string baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
                string reviewPathLink = baseUrl + Url.Action("ReviewAgreement", "Home", new { reqId = model.Reqid });

                SendEmail(model.Email, "Review Agreement", $"Hello, Review the agreement properly: {reviewPathLink}");
                _notyf.Success("Agreement Sent");
            }
            catch (Exception ex)
            {
                _notyf.Error("Failed to sent");
            }
            return RedirectToAction("AdminDashboard");
        }

        [HttpGet]
        public IActionResult CloseCase(int ReqId)
        {
            var model = _adminService.ShowCloseCase(ReqId);
            return View(model);
        }

        [HttpPost]
        public IActionResult CloseCase(CloseCaseModel model)
        {

            bool isSaved =_adminService.SaveCloseCase(model);
            if (isSaved)
            {
                _notyf.Success("Saved");
            }
            else
            {
                _notyf.Error("Failed");
            }
            return RedirectToAction("CloseCase", new {ReqId=model.reqid});
        }

        public IActionResult SubmitCloseCase(int ReqId)
        {
            bool isClosed = _adminService.SubmitCloseCase(ReqId);
            if (isClosed)
            {
                _notyf.Success("Closed Successfully");
                return RedirectToAction("AdminDashboard");
            }
            else
            {
                _notyf.Error("Failed to close");
                return RedirectToAction("CloseCase", new { ReqId = ReqId });
            }
        }

        [HttpGet]
        public IActionResult EncounterForm(int ReqId)
        {
            var model = _adminService.EncounterForm(ReqId);
            return View(model);
        }

        [HttpPost]
        public IActionResult EncounterForm(EncounterFormModel model)
        {
            bool isSaved = _adminService.SubmitEncounterForm(model);
            if (isSaved)
            {
                _notyf.Success("Saved!!");
            }
            else {
                _notyf.Error("Failed");
            }
            return RedirectToAction("EncounterForm", new { ReqId = model.reqid });
        }


        [HttpGet]
        public IActionResult ShowMyProfile() 
        {
            var request = HttpContext.Request;
            var token = request.Cookies["jwt"];
            if (token == null || !_jwtService.ValidateToken(token, out JwtSecurityToken jwtToken))
            {
                return Json("token expired");
            }
            var emailClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email);

            var model = _adminService.MyProfile(emailClaim.Value);
            return PartialView("_MyProfile",model);
        }
        public string GetTokenEmail()
        {
            var token = HttpContext.Request.Cookies["jwt"];
            if (token == null || !_jwtService.ValidateToken(token, out JwtSecurityToken jwtToken))
            {
                return "";
            }
            var emailClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email);
            return emailClaim.Value;
        }
        [HttpPost]
        public IActionResult ResetPassword(string resetPassword)
        {
            var tokenEmail = GetTokenEmail();
            if (tokenEmail != "")
            {
                resetPassword= GenerateSHA256(resetPassword);
                bool isReset = _adminService.ResetPassword(tokenEmail,resetPassword);
                return Json(new { isReset = isReset });
            }
            return Json(new { isReset = false });   
        }

        [HttpGet]
        public IActionResult CreateRequest() 
        {
            return View();        
        }

        [HttpGet]
        public IActionResult VerifyState(string stateMain)
        {
            if (stateMain==null || stateMain.Trim() == null)
            {
                return Json(new { isSend = false });
            }
            var isSend = _adminService.VerifyState(stateMain);
            return Json(new { isSend = isSend });
        }


        [HttpPost]
        public IActionResult CreateRequest(CreateRequestModel model)
        {
            var request = HttpContext.Request;
            var token = request.Cookies["jwt"];
            if (token == null || !_jwtService.ValidateToken(token, out JwtSecurityToken jwtToken))
            {
                _notyf.Error("Token Expired,Login Again");
                return View(model);
            }
            var emailClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email);
            var isSaved = _adminService.CreateRequest(model, emailClaim.Value);
            if (isSaved)
            {
                _notyf.Success("Request Created");
                return RedirectToAction("AdminDashboard");
            }
            else
            {
                _notyf.Error("Failed to Create");
                return View(model);
            }
        }


        public IActionResult ShowProvider()
        {
            return PartialView("_Provider");
        }

    }


}
