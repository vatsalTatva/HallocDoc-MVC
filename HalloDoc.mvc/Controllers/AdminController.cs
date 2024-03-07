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
        public IActionResult GetRequestsByStatus(int tabNo)
        {
            var list = _adminService.GetRequestsByStatus(tabNo);
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

        
        public IActionResult ViewCase(int Requestclientid,int RequestTypeId)
        {
            var obj = _adminService.ViewCaseViewModel(Requestclientid,RequestTypeId);

            return View(obj);
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
        public IActionResult SubmitCancelCase(int casetag, string notes,int reqId)
        {
            CancelCaseModel cancelCaseModel = new()
            {
                casetag = casetag,
                notes = notes,
                reqId = reqId
            };


            
            bool isCancelled = _adminService.SubmitCancelCase(cancelCaseModel);
                
                   // _notyf.Success("Cancelled successfully");
                    return Json(new { isCancelled = isCancelled });
             


        }

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
            HttpContext.Session.SetInt32("BlockReqId", reqId);
            var model = _adminService.BlockCase(reqId);
            return PartialView("_BlockCase",model);
        }

        [HttpPost]
        public IActionResult SubmitBlockCase(BlockCaseModel blockCaseModel)
        {
            blockCaseModel.ReqId = HttpContext.Session.GetInt32("BlockReqId");
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

            var message = string.Join(", ", selectedFiles);
            SendEmail("yashvariya23@gmail.com", "Documents", message);
            _notyf.Success("Send Mail Successfully");
            return RedirectToAction("ViewUploads", "Admin", new { reqId = rid });
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

        public IActionResult SendOrder()
        {
            return View();
        }

    }


}
